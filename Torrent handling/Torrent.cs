using OversimplifiedTorrent.Data_structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {

    [Serializable]
    public class Torrent : INotifyPropertyChanged {
        private byte[] peerID;
        private string directory;
        private TorrentMetadata torrentMetadata;
        private TrackersHandler trackersHandler;
        private DownloadingProgress downloadingProgress;
        private ValidatedAccess pieces;
        private Bitfield localBitfield;
        private PeersManager peersManager;
        private PeersConnectionsSetter connectionsSetter;

        public enum StatusType { Cheking, Pause, Downloading, Seeding }

        private StatusType status;
        public StatusType Status {
            get {
                return status;
            }
            set {
                status = value;
                OnPropertyChanged("StatusString");
            }
        }


        #region ViewProperties

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string Name {
            get {
                return torrentMetadata.name;
            }
        }

        public int PiecesCount {
            get {
                return torrentMetadata.pieces.Length / 20;
            }
        }

        private int progressBarMaximum;
        public int ProgressBarMaximum { 
            get {
                return progressBarMaximum;
            }
            private set {
                progressBarMaximum = value;
                OnPropertyChanged("ProgressBarMaximum");
            }
        }

        private int progressBarValue;
        public int ProgressBarValue {
            get {
                return progressBarValue;
            }
            private set {
                progressBarValue = value;
                OnPropertyChanged("ProgressBarValue");
            }
        }

        public string StatusString {
            get {
                switch (Status) {
                    case StatusType.Cheking:
                        return "Проверка файлов";

                    case StatusType.Downloading:
                        return "Загрузка";

                    case StatusType.Pause:
                        return "Пауза";

                    case StatusType.Seeding:
                        return "Раздача";

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public string SizeString {
            get {
                string[] sizes = { "B", "KB", "MB", "GB", "TB" };
                double len = pieces.FilesSize;
                int order = 0;
                while (len >= 1024 && order < sizes.Length - 1) {
                    order++;
                    len = len / 1024;
                }
                return String.Format("{0:0.##} {1}", len, sizes[order]);
            }
        }

        public string TorrentDirectory {
            get {
                return directory;
            }
        }

        public string CreatedBy {
            get {
                return torrentMetadata.createdBy;
            }
        }

        public DateTime CreatedDate {
            get {
                return torrentMetadata.creationDate;
            }
        }

        public string Publisher {
            get {
                return torrentMetadata.publisher;
            }
        }

        public string PublisherURL {
            get {
                return torrentMetadata.publisherURL;
            }
        }

        public BindingList<Peer> PeerList {
            get {
                return peersManager.PeerList;
            }
        }

        #endregion

        public Torrent(TorrentMetadata torrentMetadata, string directory, bool checkFiles) {
            if (IsValid(torrentMetadata)) {
                this.torrentMetadata = torrentMetadata;
                this.directory = directory;
                peerID = GetRandomID();
                downloadingProgress = new DownloadingProgress();
                CreateDirectories(torrentMetadata.files, directory);
                pieces = new ValidatedAccess(torrentMetadata.files, directory, torrentMetadata.pieceLength, torrentMetadata.pieces);
                localBitfield = new Bitfield(PiecesCount);
                localBitfield.OnRecivingPiece += CheckIfDownloaded;
                trackersHandler = new TrackersHandler(torrentMetadata.announceList, torrentMetadata.infoHash, peerID);
                peersManager = new PeersManager(torrentMetadata.infoHash, peerID, localBitfield, pieces);
                connectionsSetter = new PeersConnectionsSetter(torrentMetadata.infoHash, peerID);
                PeerHandshaker.AddPeersManager(peersManager);
                trackersHandler.RegisterPeerConnectionSetter(connectionsSetter);
                ProgressBarMaximum = pieces.PiecesCount;

                localBitfield.OnRecivingPiece += (index) => {
                    if (Status == StatusType.Downloading) {
                        ProgressBarValue += 1;
                    }
                };

                if (checkFiles) {
                    Status = StatusType.Cheking;
                    ProgressBarValue = 0;
                    Task task = new Task(() => {
                        for (int i = 0; i < pieces.PiecesCount; i++) {
                            if (pieces.Read(i) != null) {
                                localBitfield.MarkAsRecived(i);
                            }
                            ProgressBarValue += 1;
                        }
                        int dpc = localBitfield.GetDownloadedPiecesCount();
                        ProgressBarValue = dpc;                      
                        Status = StatusType.Pause;
                    });
                    task.Start();
                }

            }
            else {
                throw new Exception();
            }
        }

        private void CheckIfDownloaded(int index) {
            if (localBitfield.GetDownloadedPiecesCount() == pieces.PiecesCount) {
                Status = StatusType.Seeding;
            }
        }

        private void CreateDirectories(List<FileMetadata> files, string directory) {
            foreach (FileMetadata file in files) {
                int nameStartPos = file.relativePath.LastIndexOf('\\') + 1;
                if (nameStartPos == 0) {
                    Directory.CreateDirectory(directory + '\\');
                }
                else {
                    Directory.CreateDirectory(directory + '\\' + file.relativePath.Substring(0, nameStartPos));
                }
            }
        }

        private byte[] GetRandomID() {
            byte[] id = new byte[20];
            Random random = new Random();
            for (int i = 0; i < 16; i++) {
                id[i] = (byte)random.Next();
            }
            DateTime curtime = DateTime.Now;
            id[16] = (byte)curtime.Day;
            id[17] = (byte)curtime.Hour;
            id[18] = (byte)curtime.Minute;
            id[19] = (byte)curtime.Second;
            return id;
        }

        public static bool IsValid(TorrentMetadata torrentMetadata) {
            if ((torrentMetadata.announceList != null) && (torrentMetadata.files != null) &&
                (torrentMetadata.pieceLength > 0) && (torrentMetadata.pieces != null) &&
                (torrentMetadata.infoHash != null)) {
                return true;
            }
            return false;
        }

        public void StartDownloading() {
            if (Status == StatusType.Pause) {
                trackersHandler.StartUpdating(downloadingProgress);
                peersManager.Start();
                if (pieces.PiecesCount == localBitfield.GetDownloadedPiecesCount()) {
                    Status = StatusType.Seeding;
                }
                else {
                    Status = StatusType.Downloading;
                }
            }
        }

        public void StopDownloading() {
            trackersHandler.StopUpdating();
            peersManager.Stop();
            Status = StatusType.Pause;
        }
    }
}

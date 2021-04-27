using OversimplifiedTorrent.TorrentHandling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OversimplifiedTorrent {

    [Serializable]
    public class Torrent : INotifyPropertyChanged {

        private enum TorrentStatus { PAUSE, DOWNLOADING }

        private TorrentFilesStream torrentFileStream;
        private TorrentTrackersManager torrentTrackers;
        [NonSerialized]
        private TorrentStatus status = TorrentStatus.PAUSE;

        public string StatusString {
            get {
                switch (status) {
                    case TorrentStatus.DOWNLOADING:
                        return "Загрузка";

                    case TorrentStatus.PAUSE:
                        return "Пауза";
                }
                return "????";
            }
        }

        public string Name { get; }

        public string CreatedBy { get; }

        public DateTime CreationDate { get; }

        public string Publisher { get; }

        public string PublisherURL { get; }

        public long PieceLength { get; }

        public long Size {
            get {
                return torrentFileStream.Size;
            }
        }

        public string SizeString {
            get {
                string[] sizes = { "Б", "КБ", "МБ", "ГБ", "ТБ" };
                double len = Size;
                int order = 0;
                while (len >= 1024 && order < sizes.Length - 1) {
                    order++;
                    len = len / 1024;
                }
                return len.ToString("F") + " " + sizes[order];
            }
        }

        public long Uploaded { get; }

        public long Downloaded {
            get {
                return torrentFileStream.Downloaded;
            }
        }

        public long Left { get; }

        public string Directory {
            get {
                return torrentFileStream.Directory;
            }
        }

        public BindingList<TorrentFile> files {
            get {
                return torrentFileStream.files;
            }
        }

        public BindingList<TorrentTracker> trackers {
            get {
                return torrentTrackers.Trackers;
            }
        }

        public List<string> AnnounceURLs { get; }

        public string InfoHash { get; }

        public string PeerID { get; }

        public int Port { get; } = 6881;

        public Torrent(string torrentFilePath) {
            TorrentBencodeParser torrentBencodeData = new TorrentBencodeParser(torrentFilePath);
            Name = torrentBencodeData.Name;
            CreatedBy = torrentBencodeData.CreatedBy;
            CreationDate = torrentBencodeData.CreationDate;
            Publisher = torrentBencodeData.Publisher;
            PublisherURL = torrentBencodeData.PublisherURL;
            PieceLength = torrentBencodeData.PieceLength;
            InfoHash = torrentBencodeData.InfoHash;
            AnnounceURLs = torrentBencodeData.AnnounceList;
            PeerID = GeneratePeerID();
            torrentFileStream = new TorrentFilesStream("D:\\KSISLabs\\Coursework3\\OversimplifiedTorrent\\DownloadDIR", torrentBencodeData.Files);
            torrentTrackers = new TorrentTrackersManager(this);

            //Должны быть убраны
            Uploaded = 0;
            Left = Size;
        }

        public override string ToString() {
            return Name;
        }

        public void Continue() {
            status = TorrentStatus.DOWNLOADING;
            NotifyPropertyChanged("StatusString");

            torrentTrackers.StartUpdating();
        }

        public void Stop() {
            status = TorrentStatus.PAUSE;
            NotifyPropertyChanged("StatusString");

            torrentTrackers.StopUpdating();
        }

        private string GeneratePeerID() {
            byte[] bytes = new byte[20];
            Random random = new Random();
            for (int i = 0; i < 20; i++) {
                bytes[i] = (byte)random.Next();
            }
            return Convert.ToBase64String(bytes).Substring(0, 20);
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

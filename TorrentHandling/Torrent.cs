using OversimplifiedTorrent.TorrentHandling;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OversimplifiedTorrent {

    [Serializable]
    public class Torrent : INotifyPropertyChanged {

        
        private TorrentFilesStream torrentFileStream;
        private TorrentTrackersManager torrentTrackers;

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

        public long Downloaded {
            get {
                return torrentFileStream.Downloaded;
            }
        }

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

        public Torrent(string torrentFilePath) {
            TorrentBencodeParser torrentBencodeData = new TorrentBencodeParser(torrentFilePath);
            Name = torrentBencodeData.Name;
            CreatedBy = torrentBencodeData.CreatedBy;
            CreationDate = torrentBencodeData.CreationDate;
            Publisher = torrentBencodeData.Publisher;
            PublisherURL = torrentBencodeData.PublisherURL;
            PieceLength = torrentBencodeData.PieceLength;
            torrentFileStream = new TorrentFilesStream("D:\\KSISLabs\\Coursework3\\OversimplifiedTorrent\\DownloadDIR", torrentBencodeData.Files);
            torrentTrackers = new TorrentTrackersManager(torrentBencodeData.AnnounceList);
        }

        public override string ToString() {
            return Name;
        }

        public void Stop() {
            
        }

        public void Continue() {

        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

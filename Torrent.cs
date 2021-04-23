using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OversimplifiedTorrent {
    public class Torrent : INotifyPropertyChanged {

        public string Name { get; }
        public string CreatedBy { get; }
        public DateTime CreationDate { get; }

        public Torrent(string torrentFilePath) {
            TorrentBencodeParser torrentBencodeData = new TorrentBencodeParser(torrentFilePath);
            Name = torrentBencodeData.Name;
            CreatedBy = torrentBencodeData.CreatedBy;
            CreationDate = torrentBencodeData.CreationDate;
        }

        public override string ToString() {
            return Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

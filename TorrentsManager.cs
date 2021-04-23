using System.ComponentModel;

namespace OversimplifiedTorrent {
    public static class TorrentsManager {

        public static BindingList<Torrent> TorrentsList { get; }

        static TorrentsManager() {
            TorrentsList = new BindingList<Torrent>();
        }

        public static void Add(string torrentFilePath) {
            TorrentsList.Add(new Torrent(torrentFilePath));
        }
    }
}

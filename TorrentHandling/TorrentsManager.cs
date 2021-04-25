using System.ComponentModel;
using System.Windows;

namespace OversimplifiedTorrent {
    public static class TorrentsManager {

        public static BindingList<Torrent> TorrentsList { get; }

        static TorrentsManager() {
            TorrentsList = new BindingList<Torrent>();
        }

        public static void Add(string torrentFilePath) {
            try {
                TorrentsList.Add(new Torrent(torrentFilePath));
            }
            catch {
                MessageBox.Show("Не удалось добавить торрент файл");
            }
        }
    }
}

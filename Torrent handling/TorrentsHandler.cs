using OversimplifiedTorrent.BencodeParsing;
using System;
using System.ComponentModel;
using System.Windows;

namespace OversimplifiedTorrent {

    [Serializable]
    public static class TorrentsHandler {

        public static BindingList<Torrent> TorrentsList { get; set; }

        static TorrentsHandler() {
            TorrentsList = new BindingList<Torrent>();
        }

        public static void Add(string torrentFilePath) {
            try {
                TorrentMetadata torrentMetadata = TorrentParser.Parse(torrentFilePath);
                Torrent torrent = new Torrent(torrentMetadata, "F:\\Cursework3Playground");
                TorrentsList.Add(torrent);
            }
            catch {
                MessageBox.Show("Не удалось добавить торрент", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

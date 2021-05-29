using Microsoft.Win32;
using OversimplifiedTorrent.BencodeParsing;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;

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
                FolderBrowserDialog saveFileDialog = new FolderBrowserDialog();
                saveFileDialog.ShowDialog();
                Torrent torrent = new Torrent(torrentMetadata, saveFileDialog.SelectedPath);
                TorrentsList.Add(torrent);
            }
            catch {
                System.Windows.MessageBox.Show("Не удалось добавить торрент", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

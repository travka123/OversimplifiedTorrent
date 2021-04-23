using Microsoft.Win32;
using System.Windows;

namespace OversimplifiedTorrent {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            TorrentsList.ItemsSource = TorrentsManager.TorrentsList;
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void MenuItem_Click_Add(object sender, RoutedEventArgs e) {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Торренты (*.torrent)|*.torrent|Все файлы (*.*)|*.*";
            if (fileDialog.ShowDialog() == true) {
                TorrentsManager.Add(fileDialog.FileName);
            }
        }
    }
}

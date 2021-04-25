using Microsoft.Win32;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            TorrentData_ContextChanged();
        }

        private void TorrentsList_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            TorrentData_ContextChanged();
        }

        private void TorrentData_ContextChanged() {
            var item = TorrentsList.SelectedItem;
            if (item != null) {
                switch (TorrentDataTab.SelectedIndex) {

                    case 0:
                        ShowTorrentInfo(item as Torrent);
                        break;

                    case 1:
                        ShowTorrentFilesInfo(item as Torrent);
                        break;

                    case 2:
                        ShowTorrentTrackersInfo(item as Torrent);
                        break;
                }
            }
        }

        private void ShowTorrentInfo(Torrent torrent) {
            StringBuilder SBTorrentInfo = new StringBuilder();
            SBTorrentInfo.Append("Имя: " + torrent.Name + Environment.NewLine);
            SBTorrentInfo.Append("Каталог: " + torrent.Directory + Environment.NewLine);
            SBTorrentInfo.Append("Размер: " + torrent.SizeString + Environment.NewLine);
            SBTorrentInfo.Append("Автор: " + torrent.CreatedBy + Environment.NewLine);
            SBTorrentInfo.Append("Дата создания: " + torrent.CreationDate.ToString() + Environment.NewLine);
            SBTorrentInfo.Append("Издатель: " + (torrent.Publisher ?? "не указано") + Environment.NewLine);
            SBTorrentInfo.Append("URL издателя: " + (torrent.PublisherURL ?? "не указано") + Environment.NewLine);
            tbTorrentInfo.Text = SBTorrentInfo.ToString();
        }

        private void ShowTorrentFilesInfo(Torrent torrent) {
            TorrentFilesList.ItemsSource = torrent.files;
        }

        private void ShowTorrentTrackersInfo(Torrent torrent) {
            TorrentTrackersList.ItemsSource = torrent.trackers;
        }
    }
}

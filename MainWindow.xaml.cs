using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace OversimplifiedTorrent {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            TorrentsList.ItemsSource = TorrentsHandler.TorrentsList;
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void MenuItem_Click_Add(object sender, RoutedEventArgs e) {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Торренты (*.torrent)|*.torrent|Все файлы (*.*)|*.*";
            if (fileDialog.ShowDialog() == true) {
                TorrentsHandler.Add(fileDialog.FileName);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            TabControl_ContextChanged();
        }

        private void TorrentsList_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            TabControl_ContextChanged();
        }

        private void TabControl_ContextChanged() {
            //var item = TorrentsList.SelectedItem;
            //if (item != null) {
            //    switch (TorrentDataTab.SelectedIndex) {

            //        case 0:
            //            ShowTorrentInfo(item as TorrentOld);
            //            break;

            //        case 1:
            //            ShowTorrentFilesInfo(item as TorrentOld);
            //            break;

            //        case 2:
            //            ShowTorrentTrackersInfo(item as TorrentOld);
            //            break;
            //    }
            //}
        }

        private void ShowTorrentInfo(Torrent torrent) {
            //StringBuilder SBTorrentInfo = new StringBuilder();
            //SBTorrentInfo.Append("Имя: " + torrent.Name + Environment.NewLine);
            //SBTorrentInfo.Append("Каталог: " + torrent.Directory + Environment.NewLine);
            //SBTorrentInfo.Append("Размер: " + torrent.SizeString + Environment.NewLine);
            //SBTorrentInfo.Append("Автор: " + torrent.CreatedBy + Environment.NewLine);
            //SBTorrentInfo.Append("Дата создания: " + torrent.CreationDate.ToString() + Environment.NewLine);
            //SBTorrentInfo.Append("Издатель: " + (torrent.Publisher ?? "не указано") + Environment.NewLine);
            //SBTorrentInfo.Append("URL издателя: " + (torrent.PublisherURL ?? "не указано") + Environment.NewLine);
            //tbTorrentInfo.Text = SBTorrentInfo.ToString();
        }

        private void ShowTorrentFilesInfo(Torrent torrent) {
            //TorrentFilesList.ItemsSource = torrent.files;
        }

        private void ShowTorrentTrackersInfo(Torrent torrent) {
            //TorrentTrackersList.ItemsSource = torrent.trackers;
        }

        private void TorrentsList_PreviewMouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            TabControl_ContextChanged();
        }

        private void MenuItem_Click_DeleteTorrentFile(object sender, RoutedEventArgs e) {
            //TorrentOld torrents = TorrentsList.SelectedItem as TorrentOld;
            //torrents.Stop();
            //TorrentsHandler.TorrentsList.Remove(torrents);
            //ClearTorrentInfo();
        }

        private void ClearTorrentInfo() {
            //tbTorrentInfo.Text = "";
            //TorrentFilesList.ItemsSource = null;
            //TorrentTrackersList.ItemsSource = null;
        }

        private void MenuItem_Click_TorrentPause(object sender, RoutedEventArgs e) {
            //TorrentOld torrents = TorrentsList.SelectedItem as TorrentOld;
            //torrents.Stop();
        }

        private void MenuItem_Click_TorrentContinue(object sender, RoutedEventArgs e) {
            Torrent torrents = TorrentsList.SelectedItem as Torrent;
            torrents.StartDownloading();
        }
    }
}

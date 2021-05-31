using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace OversimplifiedTorrent {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            PeersConnectionsReciver.StartListen();
            TorrentsList.ItemsSource = TorrentsHandler.TorrentsList;
            //BinaryFormatter binaryFormatter = new BinaryFormatter();
            //try {
            //    using (FileStream fs = new FileStream("people.dat", FileMode.OpenOrCreate)) {
            //        TorrentsHandler.TorrentsList = (BindingList<Torrent>)binaryFormatter.Deserialize(fs);
            //    }
            //    TorrentsList.ItemsSource = TorrentsHandler.TorrentsList;
            //}
            //catch {
            //    TorrentsList.ItemsSource = TorrentsHandler.TorrentsList;
            //}
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
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            //using (FileStream fs = new FileStream("people.dat", FileMode.OpenOrCreate)) {
            //    binaryFormatter.Serialize(fs, TorrentsHandler.TorrentsList);
            //}
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            TabControl_ContextChanged();
        }

        private void TorrentsList_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            TabControl_ContextChanged();
        }

        private Timer updateTimer;

        private void TabControl_ContextChanged() {
            var item = TorrentsList.SelectedItem;
            if (updateTimer != null) {
                updateTimer.Dispose();
            }
            if (item != null) {
                switch (TorrentDataTab.SelectedIndex) {

                    case 0:
                        ShowTorrentInfo(item as Torrent);
                        break;

                    case 1:
                        ShowTorrentPeersInfo(item as Torrent);
                        break;

            //        case 2:
            //            ShowTorrentTrackersInfo(item as TorrentOld);
            //            break;
                }
            }
        }

        private void ShowTorrentInfo(Torrent torrent) {
            StringBuilder SBTorrentInfo = new StringBuilder();
            SBTorrentInfo.Append("Имя: " + torrent.Name + Environment.NewLine);
            SBTorrentInfo.Append("Каталог: " + torrent.TorrentDirectory + Environment.NewLine);
            SBTorrentInfo.Append("Размер: " + torrent.SizeString + Environment.NewLine);
            SBTorrentInfo.Append("Автор: " + torrent.CreatedBy + Environment.NewLine);
            SBTorrentInfo.Append("Дата создания: " + torrent.CreatedDate.ToString() + Environment.NewLine);
            SBTorrentInfo.Append("Издатель: " + (torrent.Publisher ?? "не указано") + Environment.NewLine);
            SBTorrentInfo.Append("URL издателя: " + (torrent.PublisherURL ?? "не указано") + Environment.NewLine);
            tbTorrentInfo.Text = SBTorrentInfo.ToString();
        }

        private void ShowTorrentPeersInfo(Torrent torrent) {
            updateTimer = new Timer((object obj) => {
                try {
                    StringBuilder sb = new StringBuilder();
                    foreach (var peer in torrent.PeerList) {
                        sb.Append(peer.AddressString + " " +peer.RemotePiecesString + Environment.NewLine);
                    }
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            new Action(delegate () {
                                tbPeerInfo.Text = sb.ToString();
                            }));
                }
                catch {

                }
            }, null, 0, 500);
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
            Torrent torrents = TorrentsList.SelectedItem as Torrent;
            torrents.StopDownloading();
        }

        private void MenuItem_Click_TorrentContinue(object sender, RoutedEventArgs e) {
            Torrent torrents = TorrentsList.SelectedItem as Torrent;
            torrents.StartDownloading();
        }
    }
}

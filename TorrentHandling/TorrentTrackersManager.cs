using OversimplifiedTorrent.BencodeParsing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace OversimplifiedTorrent.TorrentHandling {

    [Serializable]
    public class TorrentTracker : INotifyPropertyChanged {

        private enum ProtocolType { UNKNOWN, TCP, UDP };

        private Torrent torrent;
        private ProtocolType protocolType;
        private string announceURL;
        private long? interval = null;
        private DateTime? lastRequestTime;
        private bool firstRequest = true;

        public string Name { get; }
        public string Status { get; private set; }
        public string TimeFromLastRequestString {
            get {
                if (lastRequestTime != null) {
                    return (DateTime.Now - lastRequestTime).ToString() + " назад";
                }
                return "Обращений не было";
            }
        }
        public string ProtocolString {
            get {
                switch (protocolType) {
                    case ProtocolType.TCP:
                        return "TCP";

                    case ProtocolType.UDP:
                        return "UDP";
                }
                return "Неизвестный протокол";
            }
        }
        public List<PeerData> Peers { get; private set; }


        public TorrentTracker(string announceURL, Torrent torrent) {
            this.announceURL = announceURL;
            this.torrent = torrent;
            Name = NameFromURL(announceURL);
            protocolType = GetProtocolType(announceURL);
            Status = "Неизвестно";
            lastRequestTime = null;
        }

        public void StartUpdateLoop(CancellationToken token) {
            while (!token.IsCancellationRequested) {
                switch (protocolType) {

                    case ProtocolType.TCP:
                        DoHttpRequest();
                        Thread.Sleep((int)(interval ?? 60000));
                        break;

                    default:
                        return;
                }
            }
        }

        private void DoHttpRequest() {
            string requestURL = $"{announceURL}?info_hash={torrent.InfoHash}&peer_id={torrent.PeerID}&port={torrent.Port}" +
                $"&uploaded={torrent.Uploaded}&downloaded={torrent.Downloaded}&left={torrent.Left}";
            if (firstRequest) {
                requestURL += "&event=started";
            }
            HttpWebRequest request = WebRequest.CreateHttp(requestURL);
            Status = "Запрос";
            NotifyPropertyChanged("Status");
            WebResponse webResponse;
            try {
                webResponse = request.GetResponse();
                using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream())) {
                    string text = streamReader.ReadToEnd();
                    WebLogger.Log(requestURL + Environment.NewLine + text + Environment.NewLine);
                    HandleBencodeResponse(text);
                }         
            }
            catch {
                Status = "Превышено время ожидания";
                NotifyPropertyChanged("Status");
                WebLogger.Log(requestURL + Environment.NewLine + "Превышено время ожидания" + Environment.NewLine);
            }
        }

        private void HandleBencodeResponse(string text) {
            try {
                var parser = new TrackerResponseBencodeParser(text);
                interval = parser.Interval * 1000;
                Peers = parser.Peers;
                Status = "Соедиение установлено";
                NotifyPropertyChanged("Status");
            }
            catch {
                Status = text;
                NotifyPropertyChanged("Status");
            }
        }

        private string NameFromURL(string URL) {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            int len = URL.Length;
            for (; i < len - 1; i++) {
                if ((URL[i] == '/') && (URL[i + 1] == '/')) {
                    break;
                }
            }
            i += 2;
            for (; (i < len) && (URL[i] != '/'); i++) {
                sb.Append(URL[i]);
            }
            return sb.ToString();
        }

        private ProtocolType GetProtocolType(string URL) {
            if (Regex.IsMatch(URL, @"^http")) {
                return ProtocolType.TCP;
            } 
            else if (Regex.IsMatch(URL, @"^udp")) {
                return ProtocolType.UDP;
            }
            return ProtocolType.UNKNOWN;
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Serializable]
    public class TorrentTrackersManager {

        [NonSerialized]
        private CancellationTokenSource cancellationTokenSource;
        [NonSerialized]
        private List<Task> updatingTasks;

        private string infoHash;

        public BindingList<TorrentTracker> Trackers { get; }

        public TorrentTrackersManager(Torrent torrent) {
            Trackers = new BindingList<TorrentTracker>();
            foreach (string announeURL in torrent.AnnounceURLs) {
                Trackers.Add(new TorrentTracker(announeURL, torrent));
            }
        }

        public void StartUpdating() {
            if (updatingTasks == null) {
                cancellationTokenSource = new CancellationTokenSource();
                updatingTasks = new List<Task>();
                foreach (TorrentTracker tt in Trackers) {
                    Task task = new Task(() => tt.StartUpdateLoop(cancellationTokenSource.Token));
                    updatingTasks.Add(task);
                    task.Start();
                }
            }
        }

        public void StopUpdating() {
            if (updatingTasks != null) {
                cancellationTokenSource.Cancel();
                updatingTasks = null;
            }
        }

    }
}

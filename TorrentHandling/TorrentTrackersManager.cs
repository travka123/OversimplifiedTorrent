using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OversimplifiedTorrent.TorrentHandling {

    public class TorrentTracker {

        private enum ProtocolType { UNKNOWN, TCP, UDP };

        private ProtocolType protocolType;

        private string announceURL;

        private long interval;

        private DateTime? lastRequestTime;

        public string Name { get; }

        public string Status { get; }

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

        public TorrentTracker(string announceURL) {
            this.announceURL = announceURL;
            interval = 0;
            Name = NameFromURL(announceURL);
            protocolType = GetProtocolType(announceURL);
            Status = "Неизвестно";
            lastRequestTime = null;
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
    }

    public class TorrentTrackersManager {

        public BindingList<TorrentTracker> Trackers { get; }

        public TorrentTrackersManager(List<string> announceURLs) {
            Trackers = new BindingList<TorrentTracker>();
            foreach (string announeURL in announceURLs) {
                Trackers.Add(new TorrentTracker(announeURL));
            }
        }
    }
}

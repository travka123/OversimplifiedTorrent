using OversimplifiedTorrent.Data_structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public class TrackersHandler {
        private List<Tracker> trackers;

        public TrackersHandler(List<string> announceList, byte[] infoHash, byte[] peerID) {
            trackers = new List<Tracker>();
            foreach (string announce in announceList) {
                trackers.Add(new Tracker(announce, infoHash, peerID));
            }
        }

        public void StartUpdating(DownloadingProgress downloadProgress) {
            foreach (Tracker tracker in trackers) {
                tracker.StartUpdating(downloadProgress);
            }
        }

        public void StopUpdating() {
            foreach (Tracker tracker in trackers) {
                tracker.StopUpdating();
            }
        }
    }
}

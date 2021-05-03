using OversimplifiedTorrent.BencodeParsing;
using OversimplifiedTorrent.Data_structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    [Serializable]
    public class TrackersHandler {
        private List<Tracker> trackers;
        private Queue<PeerAddress> peers;
        private object queueAddLocker = new object();

        public TrackersHandler(List<string> announceList, byte[] infoHash, byte[] peerID) {
            trackers = new List<Tracker>();
            foreach (string announce in announceList) {
                Tracker tracker = new Tracker(announce, infoHash, peerID);
                trackers.Add(tracker);
                tracker.OnPeersReciving += RecivePeerAddresses;
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

        public PeerAddress GetPeerAddress() {
            if (peers.Count > 0) {
                return peers.Dequeue();
            }
            return null;
        }

        private void RecivePeerAddresses(List<PeerAddress> npeers) {
            lock (queueAddLocker) {
                foreach (PeerAddress np in npeers) {
                    peers.Enqueue(np);
                }
            }
        }
    }
}

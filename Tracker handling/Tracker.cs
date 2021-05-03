using OversimplifiedTorrent.BencodeParsing;
using OversimplifiedTorrent.Data_structures;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace OversimplifiedTorrent {
    [Serializable]
    public class Tracker {
        private string announce;
        private string infoHash;
        private string peerID;
        private DownloadingProgress downloadProgress;
        private DateTime? lastUpdateTime = null;
        private long interval = 0;
        private int retryingWaitTimeMul = 0;
        [NonSerialized]
        private Task updateTask = null;
        [NonSerialized]
        private CancellationTokenSource cancelTokenSource = null;

        public Tracker(string announce, byte[] infoHash, byte[] peerID) {
            this.announce = announce;
            this.infoHash = HttpUtility.UrlEncode(infoHash);
            this.peerID = HttpUtility.UrlEncode(peerID);
        }

        public void StartUpdating(DownloadingProgress downloadProgress) {
            this.downloadProgress = downloadProgress;
            if (updateTask == null) {
                cancelTokenSource = new CancellationTokenSource();
                updateTask = new Task(() => UpdateLoop(cancelTokenSource.Token));
                updateTask.Start();
            }
            else {
                throw new Exception();
            }
        }

        private void UpdateLoop(CancellationToken cancellationToken) {
            while (!cancellationToken.IsCancellationRequested) {
                WaitBeforeRequest(cancellationToken);
                if (!cancellationToken.IsCancellationRequested) {
                    Update();
                }
            }
        }

        private void WaitBeforeRequest(CancellationToken cancellationToken) {
            long waitTime = GetMillisecondsToWait();
            if (waitTime > 0) {
                cancellationToken.WaitHandle.WaitOne((int)waitTime);
            }
        }

        private void Update() {
            try {
                lastUpdateTime = DateTime.Now;
                TrackerResponse trackerResponse = MakeRequest(TrackerRequestType.Regular);
                ApplyResponse(trackerResponse);
                retryingWaitTimeMul = 0;
            }
            catch {
                interval = GetMillisecondsBeforeRetrying();
            }
        }

        private TrackerResponse MakeRequest(TrackerRequestType type) {
            TrackerRequest request = new TrackerRequest();
            request.Create(announce, infoHash, peerID, 1234.ToString(), downloadProgress.uploaded.ToString(),
                downloadProgress.downloaded.ToString(), downloadProgress.left.ToString(), type);
            return request.GetResponse();
        }

        private void ApplyResponse(TrackerResponse trackerResponse) {
            if (trackerResponse.failureReason != null) {
                throw new Exception(trackerResponse.failureReason);
            }
            if ((trackerResponse.interval == 0) || (trackerResponse.peers == null)) {
                throw new Exception();
            }

            interval = trackerResponse.interval * 1000;
        }

        private long GetMillisecondsBeforeRetrying() {
            if (retryingWaitTimeMul < 8) {
                retryingWaitTimeMul++;
            }
            return 10000 * (long)Math.Pow(retryingWaitTimeMul, 2);
        }

        private long GetMillisecondsToWait() {
            if (lastUpdateTime == null) {
                return -1;
            }
            return interval - (long)(DateTime.Now - lastUpdateTime.Value).TotalMilliseconds;
        }

        public void StopUpdating() {
            if (updateTask != null) {
                cancelTokenSource.Cancel();
            }
        }
    }
}

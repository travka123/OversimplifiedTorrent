using OversimplifiedTorrent.PeerSocketMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public class PeerInterruptionQueue {
        
        private Queue<PeerInterruption> interruptions;

        private CancellationTokenSource waiteTokenSource;

        private readonly object addLocker = new object();

        public PeerInterruptionQueue() {
            interruptions = new Queue<PeerInterruption>();
            waiteTokenSource = new CancellationTokenSource();
        }

        public void EnqueuePeerMessage(PeerMessage message) {
            Enqueue(new PeerMessageInterruption() { type = PeerInterruptionType.PeerMessage, peerMessage = message });
        }

        public void Enqueue(PeerInterruption interruption) {
            lock (addLocker) {
                interruptions.Enqueue(interruption);
                waiteTokenSource.Cancel();
                waiteTokenSource = new CancellationTokenSource();
            }
        }

        public PeerInterruption Dequeue() {
            lock (addLocker) {
                return interruptions.Dequeue();
            }
        }

        public CancellationToken GetWaitToken() {
            lock (addLocker) {
                return waiteTokenSource.Token;
            }
        }
    }
}

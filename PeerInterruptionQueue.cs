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
            if (interruption == null) {
                throw new ArgumentNullException();
            }
            lock (addLocker) {
                interruptions.Enqueue(interruption);
                waiteTokenSource.Cancel();
                waiteTokenSource = new CancellationTokenSource();
            }
        }

        public PeerInterruption Dequeue() {
            CancellationToken token;
            bool empty = false;
            if (interruptions.Count == 0) {
                lock (addLocker) {
                    if (interruptions.Count == 0) {
                        empty = true;
                        token = waiteTokenSource.Token;
                    }
                }
            }
            if (empty) {
                token.WaitHandle.WaitOne();
            }
            return interruptions.Dequeue();
        }
    }
}

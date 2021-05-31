using OversimplifiedTorrent.PeerSocketMessages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public class PeerMessagesAsyncReciver {

        private PeerMessageReader messageReader;
        private CancellationTokenSource cancellationTokenSource;

        public delegate void ReciveMessagesMethods(PeerMessage message);
        public event ReciveMessagesMethods OnPeerMessageReciving;

        public PeerMessagesAsyncReciver(Stream stream) {
            messageReader = new PeerMessageReader(stream);
        }

        public void StartRecive() {
            if (cancellationTokenSource != null) {
                throw new Exception();
            }
            cancellationTokenSource = new CancellationTokenSource();
            new Task(() => ReciveLoop(cancellationTokenSource.Token)).Start();
        }

        private void ReciveLoop(CancellationToken cancellationToken) {
            try {
                while (!cancellationToken.IsCancellationRequested) {
                    OnPeerMessageReciving(messageReader.ReadNext());
                }
            }
            catch {

            }
            finally {
                OnPeerMessageReciving(new PeerMessage { type=PeerMessageType.Closed, size=0 });
            }
        }

        public void StopRecive() {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = null;
        }
    }
}

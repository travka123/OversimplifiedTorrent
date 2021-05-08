using OversimplifiedTorrent.PeerSocketMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public class Peer {

        private TcpClient tcpClient;
        private PeerInterruptionQueue interruptionQueue;
        private PeerMessagesAsyncReciver reciver;

        public Peer(TcpClient tcpClient) {
            this.tcpClient = tcpClient;
            interruptionQueue = new PeerInterruptionQueue();
            reciver = new PeerMessagesAsyncReciver(tcpClient.GetStream());
            reciver.OnPeerMessageReciving += interruptionQueue.EnqueuePeerMessage;
            reciver.StartRecive();
            new Task(() => CommunicationLoop()).Start();
        }

        private void CommunicationLoop() {
            PeerInterruption interruption;
            do {
                interruptionQueue.GetWaitToken().WaitHandle.WaitOne();
                interruption = interruptionQueue.Dequeue();




            } while (interruption.type != PeerInterruptionType.CloseRequest);
        }
    }
}

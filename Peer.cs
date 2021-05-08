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
        private PiecePicker piecePicker;
        private Bitfield remoteBitfield;
        private PeerMessageWriter messageWriter;

        public Peer(TcpClient tcpClient, PiecePicker piecePicker) {
            this.tcpClient = tcpClient;
            this.piecePicker = piecePicker;

            remoteBitfield = new Bitfield(piecePicker.PiecesCount);
            piecePicker.RegisterRemoteBitfield(remoteBitfield);

            interruptionQueue = new PeerInterruptionQueue();
            reciver = new PeerMessagesAsyncReciver(tcpClient.GetStream());
            reciver.OnPeerMessageReciving += interruptionQueue.EnqueuePeerMessage;
            reciver.StartRecive();

            messageWriter = new PeerMessageWriter(tcpClient.GetStream());

            new Task(() => CommunicationLoop()).Start();
        }

        private void CommunicationLoop() {
            messageWriter.WriteBitfield(piecePicker.GetLocalBitfieldBytes());
            PeerInterruption interruption;
            do {
                interruptionQueue.GetWaitToken().WaitHandle.WaitOne();
                interruption = interruptionQueue.Dequeue();
                HandleInterruption(interruption);
            } while (interruption.type != PeerInterruptionType.CloseRequest);
        }

        private void HandleInterruption(PeerInterruption interruption) {
            switch (interruption.type) {
                case PeerInterruptionType.PeerMessage:
                    HandlePeerMessage((interruption as PeerMessageInterruption).peerMessage);
                    break;
            }
        }

        private void HandlePeerMessage(PeerMessage peerMessage) {
            switch (peerMessage.type) {
                case PeerMessageType.Bitfield:
                    HandleBitfieldMessage(peerMessage as BitfieldMessage);
                    break;
            }
        }

        private void HandleBitfieldMessage(BitfieldMessage bitfieldMessage) {
            remoteBitfield.SetBitfield(bitfieldMessage.bitfield);
        }
    }
}

using OversimplifiedTorrent.PeerSocketMessages;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public class Peer : INotifyPropertyChanged {
        const int requestSize = 32768;

        private TcpClient tcpClient;
        private PeerInterruptionQueue interruptionQueue;
        private PeerMessagesAsyncReciver reciver;
        private PiecePicker piecePicker;
        private Bitfield remoteBitfield;
        private PeerMessageWriter messageWriter;
        private ValidatedAccess validatedAccess;

        private int requestedPieceIndex;
        private int requestedPieceSize;
        private int recivedPieceSize;
        private MemoryStream pieceReciveStream;

        private int bufferedPieceIndex;
        private MemoryStream sendingPieceStreamBuffer;

        private Timer timer;

        public delegate void ClosingMethods(Peer sender);
        public event ClosingMethods OnClosing;

        public event PropertyChangedEventHandler PropertyChanged;

        private DateTime lastReciveTime;

        protected void OnPropertyChanged([CallerMemberName] string name = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string AddressString {
            get {
                return (tcpClient.Client.RemoteEndPoint as IPEndPoint).Address.ToString() + ":" +
                    (tcpClient.Client.RemoteEndPoint as IPEndPoint).Port.ToString();
            }
        }

        public string RemotePiecesString {
            get {
                return remoteBitfield.GetDownloadedPiecesCount() + " из " + validatedAccess.PiecesCount.ToString();
            }
        }

        public Peer(TcpClient tcpClient, PiecePicker piecePicker, ValidatedAccess validatedAccess) {
            this.tcpClient = tcpClient;
            this.piecePicker = piecePicker;
            this.validatedAccess = validatedAccess;

            remoteBitfield = new Bitfield(piecePicker.PiecesCount);
            piecePicker.RegisterRemoteBitfield(remoteBitfield);

            interruptionQueue = new PeerInterruptionQueue();
            reciver = new PeerMessagesAsyncReciver(tcpClient.GetStream());
            reciver.OnPeerMessageReciving += interruptionQueue.EnqueuePeerMessage;
            reciver.StartRecive();

            messageWriter = new PeerMessageWriter(tcpClient.GetStream());

            bufferedPieceIndex = -1;

            lastReciveTime = DateTime.Now;

            timer = new Timer((object obj) => {
                interruptionQueue.Enqueue(new PeerInterruption { type = PeerInterruptionType.OnTimer });
            }, null, 2000, 2000);
             

        }

        private void CommunicationLoop() {

            messageWriter.WriteBitfield(piecePicker.GetLocalBitfieldBytes());
            messageWriter.WriteUnchoke();
            PeerInterruption interruption;

            do {
                interruption = interruptionQueue.Dequeue();
                try {
                    if (interruption != null) {
                        HandleInterruption(interruption);
                    }
                    else {
                        interruption = new PeerInterruption() { type = PeerInterruptionType.OnTimer };
                        continue;
                    }
                }
                catch (Exception ex) {
                    SendClose();
                }
            } while (interruption.type != PeerInterruptionType.CloseRequest);
            reciver.StopRecive();
            OnClosing(this);
            tcpClient.Close();
            timer.Dispose();
        }

        public void Communicate() {
            new Task(() => CommunicationLoop()).Start();
        }

        private void HandleInterruption(PeerInterruption interruption) {
            switch (interruption.type) {
                case PeerInterruptionType.PeerMessage:
                    HandlePeerMessage((interruption as PeerMessageInterruption).peerMessage);
                    break;

                case PeerInterruptionType.PieceDownloaded:
                    HandlePiceDownloadedInterruption(interruption as PeerInterruptionPieceDownloaded);
                    break;

                case PeerInterruptionType.OnTimer:
                    HandleOnTimer();
                    break;


            }
        }

        private void HandleOnTimer() {
            if (!tcpClient.Connected) {
                SendClose();
            }

            if (((DateTime.Now - lastReciveTime).TotalMilliseconds > 5000) && (piecePicker.local.GetDownloadedPiecesCount() > 30)) {
                RequestNextPiece();
                lastReciveTime = DateTime.Now;
            }
        }

        private void HandlePeerMessage(PeerMessage peerMessage) {
            if (peerMessage == null) {
                return;
            }
            switch (peerMessage.type) {
                case PeerMessageType.Bitfield:
                    HandleBitfieldMessage(peerMessage as BitfieldMessage);
                    break;

                case PeerMessageType.Unchoke:
                    HandleUnchokeMessage();
                    break;

                case PeerMessageType.Piece:
                    HandlePieceMessage(peerMessage as PieceMessage);
                    break;

                case PeerMessageType.Have:
                    HandleHaveMessage(peerMessage as HaveMessage);
                    break;

                case PeerMessageType.Request:
                    HandleRequestMessage(peerMessage as RequestMessage);
                    break;

                case PeerMessageType.Closed:
                    SendClose();
                    break;
            }
        }

        private void HandleBitfieldMessage(BitfieldMessage bitfieldMessage) {
            remoteBitfield.SetBitfield(bitfieldMessage.bitfield);
        }

        private void HandleUnchokeMessage() {
            RequestNextPiece();
        }

        private void RequestNextPiece() {
            requestedPieceIndex = piecePicker.GetPieceToRecive(remoteBitfield);
            if (requestedPieceIndex != -1) {
                requestedPieceSize = validatedAccess.GetPieceSize(requestedPieceIndex);
                recivedPieceSize = 0;
                pieceReciveStream = new MemoryStream(requestedPieceSize);
                int sizeToRecive = requestedPieceSize - recivedPieceSize > requestSize ? requestSize : requestedPieceSize - recivedPieceSize;
                messageWriter.WriteRequest(requestedPieceIndex, 0, sizeToRecive);
            }
            if ((requestedPieceIndex == -1) &&
                (remoteBitfield.GetDownloadedPiecesCount() > piecePicker.local.GetDownloadedPiecesCount())) {
                throw new Exception();
            }
        }

        private void HandlePieceMessage(PieceMessage message) {
            lastReciveTime = DateTime.Now;
            if ((message.index == requestedPieceIndex) && (message.begin == recivedPieceSize)) {
                pieceReciveStream.Write(message.block, 0, message.block.Length);
                recivedPieceSize += message.block.Length;
                int sizeToRecive = requestedPieceSize - recivedPieceSize > requestSize ? requestSize : requestedPieceSize - recivedPieceSize;
                if (sizeToRecive > 0) {
                    messageWriter.WriteRequest(requestedPieceIndex, recivedPieceSize, sizeToRecive);
                }
                else {
                    validatedAccess.Write(pieceReciveStream.ToArray(), requestedPieceIndex);
                    RequestNextPiece();
                }
            }
        }

        public void SendPieceDownloaded(int index) {
            interruptionQueue.Enqueue(new PeerInterruptionPieceDownloaded {
                type = PeerInterruptionType.PieceDownloaded,
                index = index,
            });
        }

        private void HandlePiceDownloadedInterruption(PeerInterruptionPieceDownloaded interruption) {
            messageWriter.WriteHave(interruption.index);
        }

        private void HandleHaveMessage(HaveMessage message) {
            remoteBitfield.MarkAsRecived(message.have);
            if (requestedPieceIndex == -1) {
                RequestNextPiece();
            }
        }

        private void HandleRequestMessage(RequestMessage message) {
            byte[] buffer = new byte[message.length];
            if (message.index != bufferedPieceIndex) {
                byte[] piece = validatedAccess.Read(message.index);
                if (piece == null) {
                    return;
                }
                sendingPieceStreamBuffer = new MemoryStream(piece);
                bufferedPieceIndex = message.index;
            }
            sendingPieceStreamBuffer.Position = message.begin;
            sendingPieceStreamBuffer.Read(buffer, 0, buffer.Length);
            messageWriter.WritePiece(bufferedPieceIndex, message.begin, buffer);
        }

        public void SendClose() {
            interruptionQueue.Enqueue(new PeerInterruption { type = PeerInterruptionType.CloseRequest });
        }


    }
}

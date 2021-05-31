using OversimplifiedTorrent.PeerSocketMessages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public class PeersManager  {
        private ValidatedAccess validatedAccess;

        public byte[] InfoHash { get; }

        public byte[] PeerID { get; }

        PiecePicker piecePicker;

        Bitfield local;

        public bool IsRecivingConnections { get; private set; }

        public BindingList<Peer> PeerList { get; set; } = new BindingList<Peer>();

        public PeersManager(byte[] infoHash, byte[] peerID, Bitfield local, ValidatedAccess validatedAccess) {
            InfoHash = infoHash;
            PeerID = peerID;

            this.local = local;
            piecePicker = new PiecePicker(local);

            this.validatedAccess = validatedAccess;
            validatedAccess.OnPieceReciving += PieceDownloaded;

            IsRecivingConnections = false;
        }

        public void AddPeer(TcpClient tcpClient, HandshakeData handshakeData) {
            try {

                if (IsRecivingConnections) {
                    var peer = new Peer(tcpClient, piecePicker, validatedAccess);
                    PeerList.Add(peer);
                    peer.OnClosing += OnPeerClosing;
                    peer.Communicate();
                }
                else {
                    tcpClient.Close();
                }
            }
            catch {

            }
        }

        public void Start() {
            IsRecivingConnections = true;
        }

        public void Stop() {
            foreach (var peer in PeerList) {
                peer.SendClose();
            }
            IsRecivingConnections = false;
        }

        private void PieceDownloaded(int index) {
            local.MarkAsRecived(index);
            foreach (var peer in PeerList) {
                peer.SendPieceDownloaded(index);
            }
        }

        private void OnPeerClosing(Peer sender) {
            PeerList.Remove(sender);
        }
    }
}

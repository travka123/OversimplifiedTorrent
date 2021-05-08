using OversimplifiedTorrent.PeerSocketMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public class PeersManager {

        Dictionary<byte[], Peer> peers;

        public byte[] InfoHash { get; }

        public byte[] PeerID { get; }

        PiecePicker piecePicker;

        public PeersManager(byte[] infoHash, byte[] peerID, Bitfield local) {
            InfoHash = infoHash;
            PeerID = peerID;
            peers = new Dictionary<byte[], Peer>(new ByteArrayComparer());
            piecePicker = new PiecePicker(local);
        }

        public void AddPeer(TcpClient tcpClient, HandshakeData handshakeData) {
            peers[handshakeData.peerID] = new Peer(tcpClient, piecePicker);
        }

        public void Start() {
            
        }

        public void Stop() {
            
        }
    }
}

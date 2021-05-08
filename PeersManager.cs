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

        public PeersManager(byte[] infoHash, byte[] peerID) {
            InfoHash = infoHash;
            PeerID = peerID;
            peers = new Dictionary<byte[], Peer>(new ByteArrayComparer());
        }

        public void AddPeer(TcpClient tcpClient, HandshakeData handshakeData) {
            peers[handshakeData.peerID] = new Peer(tcpClient);
        }

        public void Start() {
            
        }

        public void Stop() {
            
        }
    }
}

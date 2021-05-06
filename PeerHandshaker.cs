using OversimplifiedTorrent.PeerMessageStream;
using OversimplifiedTorrent.PeerSocketMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public static class PeerHandshaker {

        private static Dictionary<byte[], PeersManager> peersManagers;

        static PeerHandshaker() {
            peersManagers = new Dictionary<byte[], PeersManager>(new ByteArrayComparer());
        }

        public static void HandleIncomingConnection(TcpClient tcpClient) {
            PeerHandshakeStream handshakeStream = new PeerHandshakeStream(tcpClient.GetStream());
            HandshakeData handshakeData = handshakeStream.ReadHandshake();
            PeersManager manager = peersManagers[handshakeData.infoHash];
            handshakeStream.WriteHandshake(manager.InfoHash, manager.PeerID);
            manager.AddPeer(tcpClient, handshakeData);
        }

        public static void HandleOutcomingConnection(TcpClient tcpClient, byte[] infoHash, byte[] peerID) {
            PeerHandshakeStream handshakeStream = new PeerHandshakeStream(tcpClient.GetStream());
            handshakeStream.WriteHandshake(infoHash, peerID);
            HandshakeData handshakeData = handshakeStream.ReadHandshake();
            peersManagers[handshakeData.infoHash].AddPeer(tcpClient, handshakeData);
        }

        public static void AddPeersManager(PeersManager manager) {
            peersManagers.Add(manager.InfoHash, manager);
        }
    }
}

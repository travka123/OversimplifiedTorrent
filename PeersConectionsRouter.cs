using OversimplifiedTorrent.PeerSocketMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public static class PeersConectionsRouter {

        private static Dictionary<string, PeersManager> peersManagers;

        static PeersConectionsRouter() {
            peersManagers = new Dictionary<string, PeersManager>();
        }

        public static void HandleIncomingConnection(TcpClient tcpClient) {
            HandshakeMessage message = PeerReader.ReadHandshake(tcpClient.GetStream());
            peersManagers[Encoding.ASCII.GetString(message.info_hash)].AddPeer(tcpClient, true);
        }

        public static void HandleOutcomingConnection(TcpClient tcpClient) {
            HandshakeMessage message = PeerReader.ReadHandshake(tcpClient.GetStream());
            peersManagers[Encoding.ASCII.GetString(message.info_hash)].AddPeer(tcpClient, false);
        }

        public static void RegisterPeerManager(PeersManager manager) {
            peersManagers.Add(Encoding.ASCII.GetString(manager.InfoHash), manager);
        }

        public static void UnregisterPeerManager(PeersManager manager) {
            peersManagers.Remove(Encoding.ASCII.GetString(manager.InfoHash));
        }
    }
}

using OversimplifiedTorrent.BencodeParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public class PeersConnectionsSetter {

        private byte[] infoHash;
        private byte[] PeerID;

        public PeersConnectionsSetter(byte[] infoHash, byte[] PeerID) {
            this.infoHash = infoHash;
            this.PeerID = PeerID;
        }

        public void AddAddresses(List<PeerAddress> addresses) {
            foreach (PeerAddress address in addresses) {
                try {
                    SetConnection(address);
                }
                catch {

                }
            }

        }

        private void SetConnection(PeerAddress address) {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(new IPEndPoint(IPAddress.Parse(address.ip), Convert.ToInt32(address.port)));
            PeerWriter.WriteHandshake(tcpClient.GetStream(), infoHash, PeerID);
            PeersConectionsRouter.HandleOutcomingConnection(tcpClient);
        }
    }
}

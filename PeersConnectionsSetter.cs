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
        private byte[] peerID;

        public PeersConnectionsSetter(byte[] infoHash, byte[] peerID) {
            this.infoHash = infoHash;
            this.peerID = peerID;
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
            PeerHandshaker.HandleOutcomingConnection(tcpClient, infoHash, peerID);
        }
    }
}

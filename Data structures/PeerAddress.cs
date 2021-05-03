using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent.BencodeParsing {
    [Serializable]
    public class PeerAddress {
        public string peerID;
        public string ip;
        public string port;
    }
}

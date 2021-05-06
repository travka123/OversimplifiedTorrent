using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent.PeerSocketMessages {
    public class HandshakeData {
        public string pstr;
        public byte[] reserved;
        public byte[] infoHash;
        public byte[] peerID;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent.PeerSocketMessages {
    public class HandshakeMessage {
        public string pstr;
        public byte[] reserved;
        public byte[] info_hash;
        public byte[] peer_id;
    }
}

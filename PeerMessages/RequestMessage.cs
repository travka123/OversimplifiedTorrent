using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent.PeerSocketMessages {
    public class RequestMessage : PeerMessage {
        public int index;
        public int begin;
        public int length;
    }
}

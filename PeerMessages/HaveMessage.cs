using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent.PeerSocketMessages {
    public class HaveMessage : PeerMessage {
        public int have;
    }
}

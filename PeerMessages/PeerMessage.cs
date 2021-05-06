using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent.PeerSocketMessages {

    public enum PeerMessageType { Unknown, KeepAlive, Choke, Unchoke, Intrested, NotIntrested, Have, Bitfield, Request, Piece }

    public class PeerMessage {
        public Int32 size;
        public PeerMessageType type;
    }
}

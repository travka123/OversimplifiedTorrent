using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    
    public enum PeerInterruptionType { PeerMessage, CloseRequest, PieceDownloaded, OnTimer };

    public class PeerInterruption {
        public PeerInterruptionType type;
    }
}

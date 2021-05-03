using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent.BencodeParsing {
    public class TrackerResponse {
        public string failureReason;
        public long interval;
        public int complete;
        public int incomplete;
        public string trackerID;
        public List<PeerAddress> peers;
    }
}

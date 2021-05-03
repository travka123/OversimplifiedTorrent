using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent.Data_structures {
    [Serializable]
    public class DownloadingProgress {
        public long downloaded;
        public long uploaded;
        public long left;
    }
}

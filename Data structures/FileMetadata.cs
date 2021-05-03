using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    [Serializable]
    public class FileMetadata {
        public long length;
        public string relativePath;
    }
}

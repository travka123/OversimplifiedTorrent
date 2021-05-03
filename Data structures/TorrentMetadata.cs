using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    [Serializable]
    public class TorrentMetadata {
        public List<string> announceList;
        public string comment;
        public string createdBy;
        public DateTime creationDate;
        public List<FileMetadata> files;
        public string name;
        public long pieceLength;
        public string pieces;
        public string source;
        public string publisher;
        public string publisherURL;
        public byte[] infoHash;
    }
}

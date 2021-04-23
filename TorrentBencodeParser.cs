using System;

namespace OversimplifiedTorrent {
    public class TorrentBencodeParser {

        private enum TorrentFileType { OneFile, MultiFile };

        private BencodeDictionary root;
        private TorrentFileType type;

        public string Name {
            get {
                return (root.data["info"] as BencodeDictionary).data["name"].ToString();
            }
        }

        public string CreatedBy {
            get {
                return root.data["created by"].ToString();
            }
        }

        public DateTime CreationDate {
            get {
                return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds((root.data["creation date"] as BencodeInteger).data);
            }
        }

        public TorrentBencodeParser(string torrentFilePath) {
            root = (BencodeParser.Parse(torrentFilePath) as BencodeDictionary);
            if ((root.data["info"] as BencodeDictionary).data["files"] != null) {
                type = TorrentFileType.MultiFile;
            }
            else {
                type = TorrentFileType.OneFile;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace OversimplifiedTorrent {
    public class TorrentBencodeParser {

        public class RawTorrentFileInfo {

            public List<string> Path { get; set; }

            public long Size { get; set; }
        }

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

        public string Comment {
            get {
                return root.data["comment"].ToString();
            }
        }

        public string Encoding {
            get {
                return root.data["encoding"].ToString();
            }
        }

        public string Publisher {
            get {
                try {
                    return root.data["publisher"].ToString();
                }
                catch {
                    return null;
                }
            }
        }

        public string PublisherURL {
            get {
                try {
                    return root.data["publisher-url"].ToString();
                }
                catch {
                    return null;
                }
            }
        }

        public long PieceLength {
            get {
                return ((root.data["info"] as BencodeDictionary).data["piece length"] as BencodeInteger).data;
            }
        }

        public List<string> AnnounceList {
            get {
                List<string> announceList = new List<string>();
                string MAnnounceString = (root.data["announce"] as BencodeString).ToString();
                announceList.Add(MAnnounceString);
                foreach (BencodeData BDAnnounceString in (root.data["announce-list"] as BencodeList).data) {
                    string AnnounceString = ((BDAnnounceString as BencodeList).data[0] as BencodeString).ToString();
                    if (AnnounceString != MAnnounceString) {
                        announceList.Add(AnnounceString);
                    }
                }
                return announceList;
            }
        }

        public List<RawTorrentFileInfo> Files {
            get {
                if ((root.data["info"] as BencodeDictionary).data["files"] != null) {
                    List<RawTorrentFileInfo> files = new List<RawTorrentFileInfo>();
                    List<BencodeData> benFiles = ((root.data["info"] as BencodeDictionary).data["files"] as BencodeList).data;
                    foreach (BencodeData benFile in benFiles) {
                        RawTorrentFileInfo file = new RawTorrentFileInfo();
                        file.Size = ((benFile as BencodeDictionary).data["length"] as BencodeInteger).data;
                        file.Path = new List<string>();
                        foreach (BencodeData benPath in ((benFile as BencodeDictionary).data["path"] as BencodeList).data) {
                            file.Path.Add((benPath as BencodeString).ToString());
                        }
                        files.Add(file);
                    }
                    return files;
                }
                else {
                    throw new NotImplementedException();
                }
            }
        }

        private string computedInfoHash = null;
        public string InfoHash {
            get {
                return computedInfoHash ?? ComputeInfoHash();
            }
        }

        private string ComputeInfoHash() {
            using (SHA1Managed sha1 = new SHA1Managed()) {
                byte[] hash = sha1.ComputeHash((root.data["___INFO_HAST_STRING___"] as BencodeString).data);
                var sb = new StringBuilder();
                foreach (byte b in hash) {
                    sb.Append('%');
                    sb.Append(b.ToString("x2"));
                }
                computedInfoHash = sb.ToString();
                return computedInfoHash;
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

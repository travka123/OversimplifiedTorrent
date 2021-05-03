using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent.BencodeParsing {
    public static class TorrentParser {
        public static TorrentMetadata Parse(string path) {
            Dictionary<string, object> root = BencodeParser.Parse(path) as Dictionary<string, object>;
            TorrentMetadata torrentMetadata = new TorrentMetadata();
            InsertData(torrentMetadata, root);
            return torrentMetadata;
        }

        private static void InsertData(TorrentMetadata meta, Dictionary<string, object> root) {
            SetAnnounceList(meta, root);
            SetComment(meta, root);
            SetCreatedBy(meta, root);
            SetCreationDate(meta, root);
            SetFiles(meta, root);
            SetName(meta, root);
            SetPieceLength(meta, root);
            SetPieces(meta, root);
            SetSource(meta, root);
            SetPublisher(meta, root);
            SetPublisherURL(meta, root);
            SetInfoHash(meta, root);
        }

        private static void SetInfoHash(TorrentMetadata meta, Dictionary<string, object> root) {
            try {
                meta.infoHash = root["__INFO_HASH__"] as byte[];
            }
            catch {

            }
        }

        private static void SetComment(TorrentMetadata meta, Dictionary<string, object> root) {
            try {
                meta.createdBy = BencodeParser.ExtractASCIIString(root["comment"]);
            }
            catch {

            }
        }

        private static void SetCreatedBy(TorrentMetadata meta, Dictionary<string, object> root) {
            try {
                meta.createdBy = BencodeParser.ExtractASCIIString(root["created by"]);
            }
            catch {

            }
        }

        private static void SetCreationDate(TorrentMetadata meta, Dictionary<string, object> root) {
            try {
                meta.creationDate = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds((long)root["creation date"]);
            }
            catch {

            }
        }

        private static void SetFiles(TorrentMetadata meta, Dictionary<string, object> root) {
            try {
                meta.files = new List<FileMetadata>();
                foreach (var item in BencodeParser.ExtractList((root["info"] as Dictionary<string, object>)["files"])) {
                    Dictionary<string, object> fileMeta = item as Dictionary<string, object>;
                    meta.files.Add(new FileMetadata { length = (long)fileMeta["length"], relativePath = GetRelativePath(fileMeta["path"]) });
                }
            }
            catch {
                meta.files = null;
            }
        }

        private static string GetRelativePath(object obj) {
            StringBuilder sb = new StringBuilder();
            foreach (object item in BencodeParser.ExtractList(obj)) {
                sb.Append(BencodeParser.ExtractUTF8String(item));
                sb.Append('\\');
            }
            return sb.Remove(sb.Length - 1, 1).ToString();
        }

        private static void SetName(TorrentMetadata meta, Dictionary<string, object> root) {
            try {
                meta.name = BencodeParser.ExtractASCIIString((root["info"] as Dictionary<string, object>)["name"]);
            }
            catch {

            }
        }

        private static void SetPieceLength(TorrentMetadata meta, Dictionary<string, object> root) {
            try {
                meta.pieceLength = (long)(root["info"] as Dictionary<string, object>)["piece length"];
            }
            catch {

            }
        }

        private static void SetPieces(TorrentMetadata meta, Dictionary<string, object> root) {
            try {
                meta.pieces = BencodeParser.ExtractASCIIString((root["info"] as Dictionary<string, object>)["pieces"]);
            }
            catch {

            }
        }

        private static void SetSource(TorrentMetadata meta, Dictionary<string, object> root) {
            try {
                meta.source = BencodeParser.ExtractASCIIString((root["info"] as Dictionary<string, object>)["source"]);
            }
            catch {

            }
        }

        private static void SetPublisher(TorrentMetadata meta, Dictionary<string, object> root) {
            try {
                meta.publisher = BencodeParser.ExtractASCIIString(root["publisher"]);
            }
            catch {

            }
        }

        private static void SetPublisherURL(TorrentMetadata meta, Dictionary<string, object> root) {
            try {
                meta.publisher = BencodeParser.ExtractASCIIString(root["publisher-url"]);
            }
            catch {

            }
        }

        private static void SetAnnounceList(TorrentMetadata meta, Dictionary<string, object> root) {
            try {
                meta.announceList = new List<string>();
                string announceMain = BencodeParser.ExtractASCIIString(root["announce"]);
                meta.announceList.Add(announceMain);
                foreach (object item in BencodeParser.ExtractList(root["announce-list"])) {
                    string announce = BencodeParser.ExtractASCIIString(BencodeParser.ExtractList(item)[0]);
                    if (announceMain != announce) {
                        meta.announceList.Add(announce);
                    }
                }
            }
            catch {
                meta.announceList = null;
            }
        }


    }
}

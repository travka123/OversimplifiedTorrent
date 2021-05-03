using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent.BencodeParsing {
    static class TrackerResponseParser {
        public static TrackerResponse Parse(Stream responseStream) {
            byte[] bytes = ReadFully(responseStream);
            TrackerResponse response = new TrackerResponse();
            return InsertData(response, BencodeParser.Parse(bytes) as Dictionary<string, object>);
        }

        public static byte[] ReadFully(Stream input) {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream()) {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0) {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private static TrackerResponse InsertData(TrackerResponse response, Dictionary<string, object> root) {
            InsertFailureReason(response, root);
            if (response.failureReason == null) {
                InsertInterval(response, root);
                InsertComplete(response, root);
                InsertIncomplete(response, root);
                InsertTrackerId(response, root);
                InsertPeers(response, root);
            }
            return response;
        }

        private static void InsertFailureReason(TrackerResponse response, Dictionary<string, object> root) {
            try {
                response.failureReason = BencodeParser.ExtractASCIIString(root["failure reason"]);
            }
            catch {

            }
        }

        private static void InsertInterval(TrackerResponse response, Dictionary<string, object> root) {
            try {
                response.interval = (long)root["interval"];
            }
            catch {

            }
        }

        private static void InsertComplete(TrackerResponse response, Dictionary<string, object> root) {
            try {
                response.complete = (int)(long)root["complete"];
            }
            catch {

            }
        }

        private static void InsertIncomplete(TrackerResponse response, Dictionary<string, object> root) {
            try {
                response.incomplete = (int)(long)root["incomplete"];
            }
            catch {

            }
        }

        private static void InsertTrackerId(TrackerResponse response, Dictionary<string, object> root) {
            try {
                response.trackerID = BencodeParser.ExtractASCIIString(root["tracker id"]);
            }
            catch {

            }
        }

        private static void InsertPeers(TrackerResponse response, Dictionary<string, object> root) {
            try {
                InsertPeersFromByteArray(response, root["peers"] as byte[]);
            }
            catch {
                try {
                    InsertPeersFromList(response, root["peers"] as List<object>);
                }
                catch {

                }
            }
        }

        private static void InsertPeersFromList(TrackerResponse response, List<object> lists) {
            response.peers = new List<PeerAddress>();
            foreach (object item in lists) {
                var dictionaryItem = item as Dictionary<string, object>;
                PeerAddress peerAddress = new PeerAddress();
                peerAddress.ip = BencodeParser.ExtractASCIIString(dictionaryItem["ip"]);
                peerAddress.port = BencodeParser.ExtractASCIIString(dictionaryItem["port"]);
                try {
                    peerAddress.peerID = BencodeParser.ExtractASCIIString(dictionaryItem["peer id"]);
                }
                catch {

                }
                response.peers.Add(peerAddress);
            }
        }

        private static void InsertPeersFromByteArray(TrackerResponse response, byte[] bytes) {
            response.peers = new List<PeerAddress>();
            for (int i = 0; i < bytes.Length; i += 6) {
                PeerAddress peer = new PeerAddress();
                peer.ip = $"{bytes[i]}.{bytes[i + 1]}.{bytes[i + 2]}.{bytes[i + 3]}";
                peer.port = (((bytes[i + 4]) << 8) + bytes[i + 5]).ToString();
                response.peers.Add(peer);
            }
        }
    }
}

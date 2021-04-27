using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent.BencodeParsing {

    public class PeerData {
        public string ip;
        public long port;
        public string peerID;
    }

    public class TrackerResponseBencodeParser {

        private BencodeDictionary root;
       
        public TrackerResponseBencodeParser(string text) {
            root = BencodeParser.ParseText(text) as BencodeDictionary;
        }

        public long Interval {
            get {
                return (root.data["interval"] as BencodeInteger).data; 
            }
        }

        public List<PeerData> Peers {
            get {
                List<PeerData> peers = new List<PeerData>();
                foreach (BencodeData item in (root.data["peers"] as BencodeList).data) {
                    BencodeDictionary dictionaryItem = item as BencodeDictionary;
                    PeerData peerData = new PeerData();
                    peerData.ip = (dictionaryItem.data["ip"] as BencodeString).ToString();
                    peerData.port = (dictionaryItem.data["port"] as BencodeInteger).data;
                    try {
                        peerData.peerID = (dictionaryItem.data["peerID"] as BencodeString).ToString();
                    }
                    catch {

                    }
                    peers.Add(peerData);
                }
                return peers;
            }
        }
    }
}

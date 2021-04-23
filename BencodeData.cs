using System;
using System.Collections.Generic;
using System.Text;

namespace OversimplifiedTorrent {

    public enum DataType { String, Integer, List, Dictionary }

    public abstract class BencodeData { }

    public class BencodeInteger : BencodeData {

        public Int64 data { get; set; }

        public override string ToString() {
            return data.ToString();
        }
    }

    public class BencodeString : BencodeData {

        public byte[] data { get; set; }

        public override string ToString() {
            return Encoding.UTF8.GetString(data);
        }
    }

    public class BencodeList : BencodeData {

        public List<BencodeData> data { get; set; }

        public override string ToString() {
            StringBuilder sb = new StringBuilder("{ ");
            foreach (BencodeData bd in data) {
                sb.Append(" " + bd.ToString() + ",");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.Append(" }" + Environment.NewLine).ToString();
        }
    }

    public class BencodeDictionary : BencodeData {

        public Dictionary<string, BencodeData> data { get; set; }

        public override string ToString() {
            StringBuilder sb = new StringBuilder("{ ");
            foreach (KeyValuePair<string, BencodeData> kvp in data) {
                sb.Append(" " + kvp.Key + " : " + kvp.Value.ToString() + ",");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.Append(" }" + Environment.NewLine).ToString();
        }
    }
}

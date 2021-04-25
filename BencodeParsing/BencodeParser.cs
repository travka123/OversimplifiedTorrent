using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OversimplifiedTorrent {
    public static class BencodeParser {

        public static BencodeData Parse(string filePath) {
            byte[] source = File.ReadAllBytes(filePath);
            int start = 0;
            return ParseNext(source, ref start);
        }

        private static BencodeData ParseNext(byte[] source, ref int position) {
            switch (Encoding.ASCII.GetChars(source, position, 1)[0]) {
                case 'i':
                    return ParseInteger(source, ref position);

                case 'l':
                    return ParseList(source, ref position);

                case 'd':
                    return ParseDictionary(source, ref position);

                default:
                    if (Char.IsDigit(Encoding.ASCII.GetChars(source, position, 1)[0])) {
                        return ParseString(source, ref position);
                    }
                    throw new Exception();
            }
        }

        private static BencodeData ParseInteger(byte[] source, ref int position) {
            StringBuilder sb = new StringBuilder();
            while (Encoding.ASCII.GetChars(source, ++position, 1)[0] != 'e') {
                sb.Append(Encoding.ASCII.GetChars(source, position, 1)[0]);
            }
            position++;
            return new BencodeInteger { data = Int64.Parse(sb.ToString()) };
        }

        private static BencodeData ParseString(byte[] source, ref int position) {
            StringBuilder sstrlen = new StringBuilder();
            while (Encoding.ASCII.GetChars(source, position, 1)[0] != ':') {
                sstrlen.Append(Encoding.ASCII.GetChars(source, position++, 1)[0]);
            }
            position++;

            int strlen = int.Parse(sstrlen.ToString());
            BencodeString data = new BencodeString();
            data.data = new byte[strlen];
            for (int i = 0; i < strlen; i++) {
                data.data[i] = source[position + i];
            }
            position += strlen;
            return data;
        }

        private static BencodeData ParseList(byte[] source, ref int position) {
            var BencodeDataList = new List<BencodeData>();
            position++;
            while (Encoding.ASCII.GetChars(source, position, 1)[0] != 'e') {
                BencodeDataList.Add(ParseNext(source, ref position));
            }
            position++;
            return new BencodeList() { data = BencodeDataList };
        }

        private static BencodeDictionary ParseDictionary(byte[] source, ref int position) {
            var BencodeDataDictionary = new Dictionary<string, BencodeData>();
            position++;
            while (Encoding.ASCII.GetChars(source, position, 1)[0] != 'e') {
                string key = ParseNext(source, ref position).ToString();
                BencodeData value = ParseNext(source, ref position);
                BencodeDataDictionary.Add(key, value);
            }
            position++;
            return new BencodeDictionary() { data = BencodeDataDictionary };
        }
    }
}

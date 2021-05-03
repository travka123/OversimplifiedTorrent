using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent.BencodeParsing {
    public static class BencodeParser {

        public static object Parse(string path) {
            byte[] source = File.ReadAllBytes(path);
            long offset = 0;
            return Parse(source, ref offset);
        }

        public static object Parse(byte[] source) {
            long offset = 0;
            switch (source[offset]) {
                case (byte)'i':
                    return ParseInteger(source, ref offset);

                case (byte)'l':
                    return ParseList(source, ref offset);

                case (byte)'d':
                    return ParseDictionary(source, ref offset);

                default:
                    if ((source[offset] >= (byte)'0') && (source[offset] <= (byte)'9')) {
                        return ParseString(source, ref offset);
                    }
                    throw new Exception("Неверный формат файла");
            }
        }

        public static object Parse(byte[] source, ref long offset) {
            switch (source[offset]) {
                case (byte)'i':
                    return ParseInteger(source, ref offset);

                case (byte)'l':
                    return ParseList(source, ref offset);

                case (byte)'d':
                    return ParseDictionary(source, ref offset);

                default:
                    if ((source[offset] >= (byte)'0') && (source[offset] <= (byte)'9')) {
                        return ParseString(source, ref offset);
                    }
                    throw new Exception("Неверный формат файла");
            }
        }

        public static string ExtractASCIIString(object obj) {
            return Encoding.ASCII.GetString(obj as byte[]);
        }

        public static string ExtractUTF8String(object obj) {
            return Encoding.UTF8.GetString(obj as byte[]);
        }

        public static List<object> ExtractList(object obj) {
            return obj as List<object>;
        }

        private static long ParseInteger(byte[] source, ref long offset) {
            offset++;
            StringBuilder sb = new StringBuilder();
            while (source[offset] != (byte)'e') {
                sb.Append((char)source[offset]);
                offset++;
            }
            offset++;
            return Convert.ToInt64(sb.ToString());
        }

        private static byte[] ParseString(byte[] source, ref long offset) {
            StringBuilder sb = new StringBuilder();
            while (source[offset] != ':') {
                sb.Append((char)source[offset]);
                offset++;
            }
            offset++;
            long strlen = Convert.ToInt64(sb.ToString());
            byte[] str = new byte[strlen];
            Array.Copy(source, offset, str, 0, strlen);
            offset += strlen;
            return str;
        }

        private static List<object> ParseList(byte[] source, ref long offset) {
            List<object> list = new List<object>();
            offset++;
            while (source[offset] != (byte)'e') {
                list.Add(Parse(source, ref offset));
            }
            offset++;
            return list;
        }

        private static Dictionary<string, object> ParseDictionary(byte[] source, ref long offset) {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            offset++;
            while (source[offset] != (byte)'e') {
                string key = Encoding.ASCII.GetString(Parse(source, ref offset) as byte[]);
                object value;
                if (key == "info") {
                    long infostart = offset;
                    value = Parse(source, ref offset);
                    dictionary.Add("__INFO_HASH__", GetSHA1(source, infostart, offset - infostart));
                }
                else {
                    value = Parse(source, ref offset);
                }
                dictionary.Add(key, value);
            }
            offset++;
            return dictionary;
        }

        private static object GetSHA1(byte[] source, long offset, long length) {
            using (SHA1Managed sha1 = new SHA1Managed()) {
                return sha1.ComputeHash(source, (int)offset, (int)length);
            }
        }
    }
}

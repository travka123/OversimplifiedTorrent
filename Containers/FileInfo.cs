using System.Collections.Generic;

namespace OversimplifiedTorrent {
    public class RawTorrentFileInfo {

        public List<string> Path { get; set; }

        public long Size { get; set; }
    }

    public class TorrentFile {
        public string Path { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public string SizeString {
            get {
                string[] sizes = { "Б", "КБ", "МБ", "ГБ", "ТБ" };
                double len = Size;
                int order = 0;
                while (len >= 1024 && order < sizes.Length - 1) {
                    order++;
                    len = len / 1024;
                }
                return len.ToString("F") + " " + sizes[order];
            }
        }

        public long Downloaded { get; set; }
    }
}

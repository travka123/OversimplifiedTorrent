using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using static OversimplifiedTorrent.TorrentBencodeParser;

namespace OversimplifiedTorrent {

    [Serializable]
    public class TorrentFile : INotifyPropertyChanged {
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

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Serializable]
    public class TorrentFilesStream {

        public BindingList<TorrentFile> files { get; }

        public long Size { get; }

        public long Downloaded { get; }

        public string Directory { get; }

        public TorrentFilesStream(string directory, List<RawTorrentFileInfo> rawFiles) {
            Directory = directory;
            files = new BindingList<TorrentFile>();
            Size = 0;
            Downloaded = 0;
            foreach (RawTorrentFileInfo rfile in rawFiles) {
                TorrentFile file = new TorrentFile();
                file.Downloaded = 0;
                file.Size = rfile.Size;
                Size += rfile.Size;
                file.Name = rfile.Path.Last();
                StringBuilder SBPath = new StringBuilder(directory);
                foreach (string ppart in rfile.Path) {
                    SBPath.Append('\\');
                    SBPath.Append(ppart);
                }
                file.Path = SBPath.ToString();
                files.Add(file);
            }
        }

    }
}

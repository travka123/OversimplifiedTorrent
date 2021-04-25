using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace OversimplifiedTorrent {
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

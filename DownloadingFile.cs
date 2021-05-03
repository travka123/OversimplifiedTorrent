using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {

    public class DownloadingFile {
        private object locker = new object();

        public string Path { get; }
        public long Length { get; }

        public DownloadingFile(string path, long length) {
            Path = path;
            Length = length;
        }

        public int Read(byte[] buffer, long offset, int length) {
            byte[] part = new byte[length];
            lock (locker) {
                using (FileStream fs = new FileStream(Path, FileMode.Open, FileAccess.Read)) {
                    fs.Seek(offset, SeekOrigin.Begin);
                    return fs.Read(part, 0, length);                   
                }
            }
        }

        public void Write(byte[] part, long offset) {
            lock (locker) {
                using (FileStream fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Write)) {
                    fs.Seek(offset, SeekOrigin.Begin);
                    fs.Write(part, 0, part.Length);
                }
            }
        }
    }
}

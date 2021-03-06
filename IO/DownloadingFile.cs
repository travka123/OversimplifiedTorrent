using System.IO;

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
            lock (locker) {
                using (FileStream fs = new FileStream(Path, FileMode.Open, FileAccess.Read)) {
                    fs.Seek(offset, SeekOrigin.Begin);
                    return fs.Read(buffer, 0, length);                   
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

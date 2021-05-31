using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {

    [Serializable]
    public class IndexedAccess {

        public long PieceLength { get; }

        public long FilesSize { get; }

        private DownloadingFile[] files;

        public IndexedAccess(List<FileMetadata> filesMetadata, string directory, long pieceLength) {
            files = new DownloadingFile[filesMetadata.Count];
            this.PieceLength = pieceLength;
            int i = 0;
            FilesSize = 0;
            foreach (FileMetadata metadata in filesMetadata) {
                files[i++] = new DownloadingFile(directory + '\\' + metadata.relativePath, metadata.length);
                FilesSize += metadata.length;
            }
        }

        public void Write(byte[] buffer, int index) {
            int i;
            long offset;
            GetPieceStartLocation(index, out i, out offset);
            using (MemoryStream mem = new MemoryStream(buffer)) {
                while ((mem.Length - mem.Position > 0) && (i < files.Length)) {
                    if (mem.Length - mem.Position <= files[i].Length - offset) {
                        byte[] bufferPart = new byte[mem.Length - mem.Position];
                        mem.Read(bufferPart, 0, bufferPart.Length);
                        files[i].Write(bufferPart, offset);
                    }
                    else {
                        byte[] bufferPart = new byte[files[i].Length - offset];
                        mem.Read(bufferPart, 0, bufferPart.Length);
                        files[i].Write(bufferPart, offset);
                        i++;
                        offset = 0;
                    }
                }
            }
        }

        public byte[] Read(int index) {
            int i;
            long offset;
            GetPieceStartLocation(index, out i, out offset);
            using (MemoryStream mem = new MemoryStream()) {
                while ((mem.Length < PieceLength) && (i < files.Length)) {
                    if (PieceLength - mem.Length > files[i].Length - offset) {
                        byte[] buffer = new byte[files[i].Length - offset];
                        int readed = files[i].Read(buffer, offset, buffer.Length);
                        mem.Write(buffer, 0, buffer.Length);
                        i++;
                        offset = 0;
                    }
                    else {
                        byte[] buffer = new byte[PieceLength - mem.Length];
                        files[i].Read(buffer, offset, buffer.Length);
                        mem.Write(buffer, 0, buffer.Length);
                    }
                }
                return mem.ToArray();
            }
        }

        private void GetPieceStartLocation(int index, out int i, out long offset) {
            long globalOffset = index * PieceLength;
            offset = 0;
            i = 0;
            do {
                offset += files[i++].Length;
            } while (offset <= globalOffset);
            offset -= files[--i].Length;
            offset = globalOffset - offset;
        }
    }
}

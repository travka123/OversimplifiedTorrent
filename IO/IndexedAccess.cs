using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {

    [Serializable]
    public class IndexedAccess {

        private long pieceLength;
        private DownloadingFile[] files;

        public IndexedAccess(List<FileMetadata> filesMetadata, string directory, long pieceLength) {
            files = new DownloadingFile[filesMetadata.Count];
            this.pieceLength = pieceLength;
            int i = 0;
            foreach (FileMetadata metadata in filesMetadata) {
                files[i++] = new DownloadingFile(directory + '\\' + metadata.relativePath, metadata.length);
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
                while ((mem.Length < pieceLength) && (i < files.Length)) {
                    if (pieceLength - mem.Length > files[i].Length - offset) {
                        byte[] buffer = new byte[files[i].Length - offset];
                        files[i].Read(buffer, offset, buffer.Length);
                        mem.Write(buffer, 0, buffer.Length);
                        i++;
                        offset = 0;
                    }
                    else {
                        byte[] buffer = new byte[pieceLength - mem.Length];
                        files[i].Read(buffer, offset, buffer.Length);
                        mem.Write(buffer, 0, buffer.Length);
                    }
                }
                return mem.ToArray();
            }
        }

        private void GetPieceStartLocation(int index, out int i, out long offset) {
            long globalOffset = index * pieceLength;
            offset = 0;
            i = 0;
            do {
                offset += files[i++].Length;
            } while (offset <= globalOffset);
            offset -= files[--i].Length;
            offset = globalOffset - offset;
        }

        //public PiecesAccess(List<FileMetadata> filesMetadata, string directory, long pieceLength) {
        //    files = new DownloadingFile[filesMetadata.Count];
        //    this.pieceLength = pieceLength;
        //    int i = 0;
        //    foreach (FileMetadata metadata in filesMetadata) {
        //        files[i].length = metadata.length;
        //        files[i].descriptor = new SynchronousFileAccess(directory + '\\' + metadata.relativePath);
        //        files[i].piecesCount = metadata.length / pieceLength;
        //        if (metadata.length % pieceLength > 0) { files[i].piecesCount++; }
        //        i++;
        //    }
        //}

        //public void Write(byte[] buffer, long index) {
        //    long offset;
        //    TorrentFile file = GetPieceLocation(index, out offset);
        //    file.descriptor.Write(buffer, offset);
        //}

        //public byte[] Read(long index) {
        //    long offset;
        //    long length;
        //    TorrentFile file = GetPieceLocation(index, out offset, out length);
        //    return file.descriptor.Read(offset, length);
        //}

        //private TorrentFile GetPieceLocation(long index, out long offset) {
        //    long total = 0;
        //    int i = 0;
        //    do {
        //        total += files[i++].piecesCount;
        //    } while (total <= index);
        //    total -= files[--i].piecesCount;
        //    offset = (index - total) * pieceLength;
        //    return files[i];
        //}

        //private TorrentFile GetPieceLocation(long index, out long offset, out long curPieceLength) {
        //    long total = 0;
        //    int i = 0;
        //    do {
        //        total += files[i++].piecesCount;
        //    } while (total <= index);
        //    total -= files[--i].piecesCount;
        //    offset = (index - total) * pieceLength;
        //    if (files[i].length < (offset + pieceLength)) {
        //        curPieceLength = files[i].length - offset;
        //    }
        //    else {
        //        curPieceLength = pieceLength;
        //    }
        //    return files[i];
        //}
    }
}

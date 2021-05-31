using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public class ValidatedAccess {
        private IndexedAccess access;
        private byte[] hashes;

        public delegate void PieceRecivedMethods(int index);
        public event PieceRecivedMethods OnPieceReciving;

        public int PiecesCount {
            get {
                return hashes.Length / 20;
            }
        }

        public long FilesSize {
            get {
                return access.FilesSize;
            }
        }

        public ValidatedAccess(List<FileMetadata> filesMetadata, string directory, long pieceLength, byte[] pieces) {
            access = new IndexedAccess(filesMetadata, directory, pieceLength);
            hashes = pieces;
        }

        public bool Write(byte[] buffer, int index) {
            using (SHA1Managed sha = new SHA1Managed()) {
                if (IsCorrectHash(index, sha.ComputeHash(buffer))) {
                    access.Write(buffer, index);
                    OnPieceReciving(index);
                    return true;
                }
                return false;
            }
        }

        public byte[] Read(int index) {
            try {
                byte[] buffer = access.Read(index);
                using (SHA1Managed sha = new SHA1Managed()) {
                    if (IsCorrectHash(index, sha.ComputeHash(buffer))) {
                        return buffer;
                    }
                    return null;
                }
            }
            catch {
                return null;
            }
        }

        private bool IsCorrectHash(int index, byte[] hash) {
            long offset = index * 20;
            for (int i = 0; i < 20; i++) {
                if (hashes[offset + i] != hash[i]) {
                    return false;
                }
            }
            return true;
        }

        public int GetPieceSize(int index) {
            if (index < PiecesCount - 1) {
                return (int)access.PieceLength;
            }
            return (int)(access.FilesSize % access.PieceLength);
        }
    }
}

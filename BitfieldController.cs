using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public class BitfieldController {

        private byte[] localBitfield;
        private bool[] recived;
        private Dictionary<string, byte[]> remoteBitfields;
        private int[] swarmPiecesAvailability;
        private Random random = new Random();

        public int PiecesCount { get; }

        public BitfieldController(int piecesCount) {
            PiecesCount = piecesCount;
            recived = new bool[piecesCount];
            localBitfield = new byte[piecesCount / 8 + (((piecesCount % 8) > 0) ? (1) : (0))];
            remoteBitfields = new Dictionary<string, byte[]>();
            swarmPiecesAvailability = new int[piecesCount];
        }

        private object localReciveLocker = new object();
        public void MarkLocalRecivedPiece(int index) {
            lock (localReciveLocker) {
                MarkRecivedPiece(index, localBitfield);
            }
            recived[index] = true;
        }

        public void MrakRemoteRecivedPiece(string peerIdentificator, int index) {
            if (!remoteBitfields.ContainsKey(peerIdentificator)) {
                remoteBitfields[peerIdentificator] = new byte[PiecesCount / 8 + (((PiecesCount % 8) > 0) ? (1) : (0))];
            }
            MarkRecivedPiece(index, remoteBitfields[peerIdentificator]);
        }

        public void MarkRemoteBitfield(string peerIdentificator, byte[] bitfield) {
            remoteBitfields[peerIdentificator] = bitfield;
            for (int i = 0; i < bitfield.Length; i++) {
                swarmPiecesAvailability[i * 8] += bitfield[i] & 128;
                swarmPiecesAvailability[i * 8 + 1] += bitfield[i] & 64;
                swarmPiecesAvailability[i * 8 + 2] += bitfield[i] & 32;
                swarmPiecesAvailability[i * 8 + 3] += bitfield[i] & 16;
                swarmPiecesAvailability[i * 8 + 4] += bitfield[i] & 8;
                swarmPiecesAvailability[i * 8 + 5] += bitfield[i] & 4;
                swarmPiecesAvailability[i * 8 + 6] += bitfield[i] & 2;
                swarmPiecesAvailability[i * 8 + 7] += bitfield[i] & 1;
            }
        }

        private int bufferSize = 10;

        public int GetPieceToRecive() {
            int[] index = new int[bufferSize];
            int[] repeats = new int[bufferSize];
            int maxRepeats = swarmPiecesAvailability[0];
            int start = 0;
            int fill = 0;
            while ((start < PiecesCount) && (fill < bufferSize)) {
                if (!recived[start]) {
                    index[fill] = start;
                    repeats[fill] = swarmPiecesAvailability[start];
                    fill++;
                }
                start++;
            }
            if (start == PiecesCount) {
                if (fill - 1 < 0) {
                    return -1;
                }
                return index[random.Next(0, fill - 1)];
            }
            for (int i = start; i < PiecesCount; i++) {
                if ((!recived[i]) && (maxRepeats > swarmPiecesAvailability[i])) {
                    for (int j = 0; j < bufferSize; j++) {
                        if (repeats[j] == maxRepeats) {
                            index[j] = i;
                            repeats[j] = swarmPiecesAvailability[i];
                            break;
                        }
                    }
                    maxRepeats = repeats[0];
                    for (int j = 0; j < bufferSize; j++) {
                        if (maxRepeats < repeats[j]) {
                            maxRepeats = repeats[j];
                        }
                    }
                }
            }
            return index[random.Next(0, bufferSize - 1)];
        }

        public int GetPieceToRecive(bool[] pending) {
            int[] index = new int[bufferSize];
            int[] repeats = new int[bufferSize];
            int maxRepeats = swarmPiecesAvailability[0];
            int start = 0;
            int fill = 0;
            while ((start < PiecesCount) && (fill < bufferSize)) {
                if ((!recived[start]) && (!pending[start])) {
                    index[fill] = start;
                    repeats[fill] = swarmPiecesAvailability[start];
                    fill++;
                }
                start++;
            }
            if (start == PiecesCount) {
                if (fill - 1 < 0) {
                    return -1;
                }
                return index[random.Next(0, fill - 1)];
            }
            for (int i = start; i < PiecesCount; i++) {
                if ((!recived[i]) && (maxRepeats > swarmPiecesAvailability[i]) && (!pending[start])) {
                    for (int j = 0; j < bufferSize; j++) {
                        if (repeats[j] == maxRepeats) {
                            index[j] = i;
                            repeats[j] = swarmPiecesAvailability[i];
                            break;
                        }
                    }
                    maxRepeats = repeats[0];
                    for (int j = 0; j < bufferSize; j++) {
                        if (maxRepeats < repeats[j]) {
                            maxRepeats = repeats[j];
                        }
                    }
                }
            }
            return index[random.Next(0, bufferSize - 1)];
        }

        private void MarkRecivedPiece(int index, byte[] bitfield) {
            int byteNumber = index / 8;
            int bitNumber = 7 - index % 8;
            byte mask = (byte)(1 << bitNumber);
            if ((bitfield[byteNumber] & mask) == 0) {
                bitfield[byteNumber] |= mask;
                swarmPiecesAvailability[index]++;
            }
        }
    }
}

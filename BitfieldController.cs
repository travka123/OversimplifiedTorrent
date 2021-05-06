using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public class BitfieldController {

        
        private bool[] recived;
        private Dictionary<string, bool[]> remoteBitfields;
        private int[] swarmPiecesAvailability;
        private Random random = new Random();

        public byte[] localBitfield { get; }

        public int PiecesCount { get; }

        public BitfieldController(int piecesCount) {
            PiecesCount = piecesCount;
            recived = new bool[piecesCount];
            localBitfield = new byte[piecesCount / 8 + (((piecesCount % 8) > 0) ? (1) : (0))];
            remoteBitfields = new Dictionary<string, bool[]>();
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
                remoteBitfields[peerIdentificator] = new bool[PiecesCount];
            }
            remoteBitfields[peerIdentificator][index] = true;
        }

        public void MarkRemoteBitfield(string peerIdentificator, byte[] bitfield) {
            remoteBitfields[peerIdentificator] = new bool[PiecesCount];
            int i;
            for (i = 0; i < bitfield.Length - 1; i++) {
                remoteBitfields[peerIdentificator][i * 8] = (bitfield[i] & 128) == 1;
                remoteBitfields[peerIdentificator][i * 8 + 1] = (bitfield[i] & 64) == 1;
                remoteBitfields[peerIdentificator][i * 8 + 2] = (bitfield[i] & 32) == 1;
                remoteBitfields[peerIdentificator][i * 8 + 3] = (bitfield[i] & 16) == 1;
                remoteBitfields[peerIdentificator][i * 8 + 4] = (bitfield[i] & 8) == 1;
                remoteBitfields[peerIdentificator][i * 8 + 5] = (bitfield[i] & 4) == 1;
                remoteBitfields[peerIdentificator][i * 8 + 6] = (bitfield[i] & 2) == 1;
                remoteBitfields[peerIdentificator][i * 8 + 7] = (bitfield[i] & 1) == 1;
            }
            byte lastbyte = bitfield[i];
            for (int j = i * 8; j < remoteBitfields[peerIdentificator].Length; j++) {
                remoteBitfields[peerIdentificator][j] = (bitfield[i] & 128) == 1;
                lastbyte <<= 1;
            }
            for (int j = 0; j < swarmPiecesAvailability.Length; j++) {
                if (remoteBitfields[peerIdentificator][j]) {
                    swarmPiecesAvailability[j]++;
                }
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

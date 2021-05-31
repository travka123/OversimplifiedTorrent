using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public class SwarmPiecesCounter {

        int[] swarmPiecesAvailability;

        public SwarmPiecesCounter(int piecesCount) {
            swarmPiecesAvailability = new int[piecesCount];
        }

        public void CountPiece(int index) {
            swarmPiecesAvailability[index]++;
        }

        public void RegisterBitfield(Bitfield bitfield) {
            CountPieces(bitfield);
            bitfield.OnRecivingPiece += CountPiece;
            bitfield.OnBitfieldChange += CountBitfieldReset;
        }

        public void CountPieces(Bitfield bitfield) {
            int i = 0;
            foreach (bool bit in bitfield) {
                if (bit) {
                    swarmPiecesAvailability[i]++;
                }
                i++;
            }
        }

        public void CountBitfieldReset(BitArray oldBitArray, BitArray newBitArray) {
            for (int i = 0; i < oldBitArray.Length; i++) {
                if (oldBitArray[i] != newBitArray[i]) {
                    if (newBitArray[i]) {
                        swarmPiecesAvailability[i]++;
                    }
                    else {
                        swarmPiecesAvailability[i]--;
                    }
                }
            }
        }

        public int GetPieceToRecive(Bitfield local, Bitfield remote, BitArray requested) {
            BitArray suitable = new BitArray(local.bitArray).Or(requested).Not().And(remote.bitArray);
            int index = GetPieceToRecive(suitable);
            if (index == -1) {
                suitable = new BitArray(local.bitArray).Not().And(remote.bitArray);
                index = GetPieceToRecive(suitable);
            }
            return index;
        }

        private Random random = new Random();

        private int GetPieceToRecive(BitArray suitable) {
            int start = random.Next(suitable.Length);
            int i = start;
            for (; (i < suitable.Length) && (!suitable[i]); i++) { }
            if (i == suitable.Length) {
                for (i = 0; (i < start) && (!suitable[i]); i++) { }
                if (i == start) {
                    return -1;
                }
            }
            int best = i;
            if (i >= start) {
                for (; i < suitable.Length; i++) {
                    if (suitable[i] && (swarmPiecesAvailability[best] > swarmPiecesAvailability[i])) {
                        best = i;
                    }
                }
            }
            for (i = 0; i < start; i++) {
                if (suitable[i] && (swarmPiecesAvailability[best] > swarmPiecesAvailability[i])) {
                    best = i;
                }
            }
            return best;
        }
    }
}

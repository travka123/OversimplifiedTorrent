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
            BitArray suitable = remote.bitArray.And(local.bitArray.Not().And(requested.Not()));
            int i;
            int minpos = -1;
            for (i = 0; i < suitable.Length; i++) {
                if (suitable[i]) {
                    minpos = i;
                    break;
                }
            }
            if (minpos == -1) {
                return -1;
            }
            for (; i < suitable.Length; i++) {
                if (suitable[i] && (swarmPiecesAvailability[minpos] > swarmPiecesAvailability[i])) {
                    minpos = i;
                }
            }
            return minpos;
        }
    }
}

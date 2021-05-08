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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public class PiecePicker {

        private BitfieldController bitfield;
        private bool[] pending;
        private bool endgame;

        public PiecePicker(BitfieldController bitfield) {
            this.bitfield = bitfield;
            pending = new bool[bitfield.PiecesCount];
            endgame = false;
        }

        public int GetPieceToRecive() {
            if (endgame) {
                return bitfield.GetPieceToRecive();
            }
            else {
                return bitfield.GetPieceToRecive(pending);
            }
        }

        public void DenyPieceReciving(int index) {
            pending[index] = false;
        }
    }
}

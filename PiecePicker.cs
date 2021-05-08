using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public class PiecePicker {

        private Bitfield local;
        private Bitfield requested;
        private SwarmPiecesCounter piecesCounter;

        public int PiecesCount {
            get {
                return local.Length;
            }
        }

        public PiecePicker(Bitfield local) {
            this.local = local;
            requested = new Bitfield(local.Length);
            piecesCounter = new SwarmPiecesCounter(local.Length);
        }

        public void RegisterRemoteBitfield(Bitfield bitfield) {
            piecesCounter.RegisterBitfield(bitfield);
        }

        public byte[] GetLocalBitfieldBytes() {
            return local.GetBytes();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public class PiecePicker {

        private Bitfield local;
        private SwarmPiecesCounter piecesCounter;
        private BitArray requested;

        private readonly object locker = new object();

        public int PiecesCount {
            get {
                return local.Length;
            }
        }

        public PiecePicker(Bitfield local) {
            this.local = local;
            requested = new BitArray(local.Length);
            piecesCounter = new SwarmPiecesCounter(local.Length);
        }

        public void RegisterRemoteBitfield(Bitfield bitfield) {
            piecesCounter.RegisterBitfield(bitfield);
        }

        public byte[] GetLocalBitfieldBytes() {
            return local.GetBytes();
        }

        public int GetPieceToRecive(Bitfield remote) {
            lock (locker) {
                int index = piecesCounter.GetPieceToRecive(local, remote, requested);
                requested[index] = true;
                return index;
            }
        }

        public void DenyFromReciving(int index) {
            requested[index] = false;
        }
    }
}

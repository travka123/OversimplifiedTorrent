using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public class Bitfield : IEnumerable {

        private BitArray bitArray;

        public int Length {
            get {
                return bitArray.Length;
            }
        } 

        public delegate void RecivePieceMethods(int index);
        public event RecivePieceMethods OnRecivingPiece;

        public delegate void BitfieldChangeMethods(BitArray oldarr, BitArray newarr);
        public event BitfieldChangeMethods OnBitfieldChange;

        public Bitfield(int piecesCount) {
            bitArray = new BitArray(piecesCount);
        }

        public Bitfield(byte[] bytes) {
            bitArray = new BitArray(bytes);
        }

        public void MarkAsRecived(int index) {
            if (!bitArray[index]) {
                bitArray[index] = true;
                OnRecivingPiece(index);
            }        
        }

        public void SetBitfield(byte[] bytes) {
            BitArray NewBitArray = new BitArray(bytes);
            NewBitArray.Length = bitArray.Length;
            BitArray oldBitArray = bitArray;
            bitArray = NewBitArray;
            OnBitfieldChange(oldBitArray, NewBitArray);
        }

        public byte[] GetBytes() {
            byte[] bytes = new byte[(bitArray.Length - 1) / 8 + 1];
            bitArray.CopyTo(bytes, 0);
            return bytes;
        }

        public IEnumerator GetEnumerator() {
            return bitArray.GetEnumerator();
        }
    }
}

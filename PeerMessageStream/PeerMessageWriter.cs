using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace OversimplifiedTorrent {

    public class PeerMessageWriter {

        private BinaryWriter binaryWriter;

        public Stream BaseStream {
            get {
                return binaryWriter.BaseStream;
            }
        }

        public PeerMessageWriter(Stream stream) {
            binaryWriter = new BinaryWriter(stream);
        }

        public void WriteKeepAlive() {
            binaryWriter.Write(0);
        }

        public void WriteBitfield(byte[] bitfield) {
            binaryWriter.Write(bitfield.Length + 1);
            binaryWriter.Write((byte)5);
            binaryWriter.Write(bitfield);
        }

        public void WriteChoke() {
            binaryWriter.Write(1);
            binaryWriter.Write((byte)0);
        }

        public void WriteUnchoke() {
            binaryWriter.Write(1);
            binaryWriter.Write((byte)1);
        }

        public void WriteInterested() {
            binaryWriter.Write(1);
            binaryWriter.Write((byte)2);
        }

        public void WriteNotInterested() {
            binaryWriter.Write(1);
            binaryWriter.Write((byte)3);
        }

        public void WriteHave(Int32 index) {
            binaryWriter.Write(5);
            binaryWriter.Write((byte)4);
            binaryWriter.Write(index);
        }

        public void WriteRequest(Int32 index, Int32 begin, Int32 length) {
            binaryWriter.Write(13);
            binaryWriter.Write((byte)6);
            binaryWriter.Write(index);
            binaryWriter.Write(begin);
            binaryWriter.Write(length);
        }

        public void WritePiece(Int32 index, Int32 begin, byte[] block) {
            binaryWriter.Write(9 + block.Length);
            binaryWriter.Write((byte)7);
            binaryWriter.Write(index);
            binaryWriter.Write(begin);
            binaryWriter.Write(block);
        }
    }
}

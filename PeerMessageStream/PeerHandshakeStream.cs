using OversimplifiedTorrent.PeerSocketMessages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent.PeerMessageStream {

    public class PeerHandshakeStream {

        BinaryReader binaryReader;
        BinaryWriter binaryWriter;
        
        public PeerHandshakeStream(Stream stream) {
            binaryReader = new BinaryReader(stream);
            binaryWriter = new BinaryWriter(stream);
        }

        public void WriteHandshake(byte[] infoHash, byte[] peerID) {
            binaryWriter.Write((byte)19);
            binaryWriter.Write(Encoding.ASCII.GetBytes("BitTorrent protocol"));
            binaryWriter.Write(new byte[8]);
            binaryWriter.Write(infoHash);
            binaryWriter.Write(peerID);
        }

        public HandshakeData ReadHandshake() {
            HandshakeData message = new HandshakeData();
            byte pstrlen = binaryReader.ReadByte();
            message.pstr = Encoding.ASCII.GetString(binaryReader.ReadBytes(pstrlen));
            message.reserved = binaryReader.ReadBytes(8);
            message.infoHash = binaryReader.ReadBytes(20);
            message.peerID = binaryReader.ReadBytes(20);
            return message;
        }
    }
}

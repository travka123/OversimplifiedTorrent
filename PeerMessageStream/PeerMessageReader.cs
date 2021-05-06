using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using OversimplifiedTorrent.PeerSocketMessages;

namespace OversimplifiedTorrent {
    public class PeerMessageReader {

        private BinaryReader binaryReader;

        public Stream BaseStream {
            get {
                return binaryReader.BaseStream;
            }
        }

        public PeerMessageReader(Stream stream) {
            binaryReader = new BinaryReader(stream);
        }

        public PeerMessage ReadNext() {
            Int32 size = binaryReader.ReadInt32();
            if (size == 0) {
                return new PeerMessage { size = size, type = PeerMessageType.KeepAlive };
            }
            byte id = binaryReader.ReadByte();
            return ReadMessageByID(size, id);
        }

        private PeerMessage ReadMessageByID(int size, byte id) {
            switch (id) {
                case 0:
                    return new PeerMessage { size = size, type = PeerMessageType.Choke };

                case 1:
                    return new PeerMessage { size = size, type = PeerMessageType.Unchoke };

                case 2:
                    return new PeerMessage { size = size, type = PeerMessageType.Intrested };

                case 3:
                    return new PeerMessage { size = size, type = PeerMessageType.NotIntrested };

                case 4:
                    return ReadHaveMessage(size, id);

                case 5:
                    return ReadBitefieldMessage(size, id);

                case 6:
                    return ReadRequestMessage(size, id);

                case 7:
                    return ReadPieceMessage(size, id);

                default:
                    binaryReader.ReadBytes(size - 1);
                    return new PeerMessage { size = size, type = PeerMessageType.Unknown };
            }
        }

        private PieceMessage ReadPieceMessage(int size, byte id) {
            PieceMessage message = new PieceMessage();
            message.size = size;
            message.type = PeerMessageType.Piece;
            message.index = binaryReader.ReadInt32();
            message.begin = binaryReader.ReadInt32();
            message.block = binaryReader.ReadBytes(size - 9);
            return message;
        }

        private RequestMessage ReadRequestMessage(int size, byte id) {
            RequestMessage message = new RequestMessage();
            message.size = size;
            message.type = PeerMessageType.Request;
            message.index = binaryReader.ReadInt32();
            message.begin = binaryReader.ReadInt32();
            message.length = binaryReader.ReadInt32();
            return message;
        }

        private BitfieldMessage ReadBitefieldMessage(int size, byte id) {
            return new BitfieldMessage { size = size, type = PeerMessageType.Bitfield, bitfield = binaryReader.ReadBytes(size - 1) };
        }

        private HaveMessage ReadHaveMessage(int size, byte id) {
            return new HaveMessage { size = size, type = PeerMessageType.Have, have = binaryReader.ReadInt32() };
        }
    }
}

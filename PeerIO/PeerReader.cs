using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using OversimplifiedTorrent.PeerSocketMessages;

namespace OversimplifiedTorrent {
    public static class PeerReader {

        public static HandshakeMessage ReadHandshake(NetworkStream stream) {
            HandshakeMessage message = new HandshakeMessage();
            BinaryReader reader = new BinaryReader(stream);
            byte pstrlen = reader.ReadByte();
            message.pstr = Encoding.ASCII.GetString(reader.ReadBytes(pstrlen));
            message.reserved = reader.ReadBytes(8);
            message.info_hash = reader.ReadBytes(20);
            message.peer_id = reader.ReadBytes(20);
            return message;
        }

        //public PeerSocketMessage ReadNext() {

        //}



    }
}

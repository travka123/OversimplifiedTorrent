using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public static class PeerWriter {
        public static void WriteHandshake(NetworkStream stream, byte[] infoHash, byte[] peerID) {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write((byte)19);
            writer.Write(Encoding.ASCII.GetBytes("BitTorrent protocol"));
            writer.Write(new byte[8]);
            writer.Write(infoHash);
            writer.Write(peerID);
        }
    }
}

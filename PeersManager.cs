﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public class PeersManager {

        public byte[] InfoHash { get; }
        public byte[] PeerID { get; }

        public PeersManager(byte[] infoHash, byte[] peerID) {
            InfoHash = infoHash;
            PeerID = peerID;
        }

        public void AddPeer(TcpClient tcpClient, bool needOutcomingHandshake) {
            if (needOutcomingHandshake) {
                PeerWriter.WriteHandshake(tcpClient.GetStream(), InfoHash, PeerID);
            }
            //Добавление пира
        }
    }
}
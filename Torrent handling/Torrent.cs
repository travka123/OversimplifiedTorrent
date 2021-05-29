﻿using OversimplifiedTorrent.Data_structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {

    [Serializable]
    public class Torrent {
        private byte[] peerID;
        private string directory;
        private TorrentMetadata torrentMetadata;
        private TrackersHandler trackersHandler;
        private DownloadingProgress downloadingProgress;
        private ValidatedAccess pieces;
        private Bitfield localBitfield;
        private PeersManager peersManager;
        private PeersConnectionsSetter connectionsSetter;
        

        #region ViewProperties
        
        public string Name {
            get {
                return torrentMetadata.name;
            }
        }

        public int PiecesCount {
            get {
                return torrentMetadata.pieces.Length / 20;
            }
        }

        #endregion

        public Torrent(TorrentMetadata torrentMetadata, string directory) {
            if (IsValid(torrentMetadata)) {
                this.torrentMetadata = torrentMetadata;
                this.directory = directory;
                peerID = GetRandomID();
                downloadingProgress = new DownloadingProgress();
                CreateDirectories(torrentMetadata.files, directory);
                pieces = new ValidatedAccess(torrentMetadata.files, directory, torrentMetadata.pieceLength, torrentMetadata.pieces);
                localBitfield = new Bitfield(PiecesCount);
                LookForDownloadedFiles(pieces, localBitfield);
                trackersHandler = new TrackersHandler(torrentMetadata.announceList, torrentMetadata.infoHash, peerID);
                peersManager = new PeersManager(torrentMetadata.infoHash, peerID, localBitfield);
                connectionsSetter = new PeersConnectionsSetter(torrentMetadata.infoHash, peerID);
                PeerHandshaker.AddPeersManager(peersManager);
                trackersHandler.RegisterPeerConnectionSetter(connectionsSetter);
            }
            else {
                throw new Exception();
            }
        }

        private void LookForDownloadedFiles(ValidatedAccess validatedAccess, Bitfield bitfield) {
            for (int i = 0; i < validatedAccess.PiecesCount; i++) {
                if (validatedAccess.Read(i) != null) {
                    bitfield.MarkAsRecived(i);
                }
            }
        }

        private void CreateDirectories(List<FileMetadata> files, string directory) {
            foreach (FileMetadata file in files) {
                int nameStartPos = file.relativePath.LastIndexOf('\\') + 1;
                if (nameStartPos == 0) {
                    Directory.CreateDirectory(directory + '\\');
                }
                else {
                    Directory.CreateDirectory(directory + '\\' + file.relativePath.Substring(0, nameStartPos));
                }
            }
        }

        private byte[] GetRandomID() {
            byte[] id = new byte[20];
            Random random = new Random();
            for (int i = 0; i < 16; i++) {
                id[i] = (byte)random.Next();
            }
            DateTime curtime = DateTime.Now;
            id[16] = (byte)curtime.Day;
            id[17] = (byte)curtime.Hour;
            id[18] = (byte)curtime.Minute;
            id[19] = (byte)curtime.Second;
            return id;
        }

        public static bool IsValid(TorrentMetadata torrentMetadata) {
            if ((torrentMetadata.announceList != null) && (torrentMetadata.files != null) &&
                (torrentMetadata.pieceLength > 0) && (torrentMetadata.pieces != null) &&
                (torrentMetadata.infoHash != null)) {
                return true;
            }
            return false;
        }

        public void StartDownloading() {        
            trackersHandler.StartUpdating(downloadingProgress);
            peersManager.Start();
        }

        

        public void StopDownloading() {
            trackersHandler.StopUpdating();
            peersManager.Stop();
        }
    }
}

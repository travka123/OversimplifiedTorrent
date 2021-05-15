using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {

    public struct PiecePartData {
        int index;
        int begin;
        int length;
    } 

    public class PiecesPartsManager {

        PiecePicker piecePicker;
        Bitfield remoteBitfield;
        ValidatedAccess validatedAccess;

        byte[] piece;
        List<PiecePartData> requestedParts;
        int pieceIndex;
        int curPieceSize;
        int unrequestedSize;

        public PiecesPartsManager(PiecePicker piecePicker, Bitfield remoteBitfield, ValidatedAccess validatedAccess) {
            this.piecePicker = piecePicker;
            this.remoteBitfield = remoteBitfield;
            this.validatedAccess = validatedAccess;

            requestedParts = new List<PiecePartData>();
            unrequestedSize = 0;
        }

        public PiecePartData GetPartToRecive() {
            
        }
    }
}

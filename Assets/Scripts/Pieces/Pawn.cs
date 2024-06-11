using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    private bool firstMove = true;
    public static Vector2Int[] directions = new Vector2Int[]{
        new Vector2Int(0, 1),
    };

    public static Vector2Int[] captureDirections = new Vector2Int[]{
        new Vector2Int(1, 1),
        new Vector2Int(-1, 1),
    };

    public override List<Vector2Int> CalculateAvailableMoves(bool alterCurrentAvailMovesList = true) {
        int maxStep = firstMove ? 2 : 1;
        int colorMult = isWhite ? 1: -1;

        // Forward movements
        List<Vector2Int> availableMoves = new List<Vector2Int>();
        foreach (var direction in directions) {
            for (int step = 1; step <= maxStep; step++) {
                Vector2Int endPos = pos + direction * step * colorMult;
                if (0 <= endPos.x && endPos.x < 8 && 0 <= endPos.y && endPos.y < 8) {
                    Piece piece = controller.getPieceAtPos(endPos);
                    if (piece == null){
                        availableMoves.Add(endPos);
                    } else {
                        break;
                    }
                }
            }
        }

        foreach (var direction in captureDirections) {
            Vector2Int endPos = pos + new Vector2Int(direction.x, direction.y * colorMult);
            if (0 <= endPos.x && endPos.x < 8 && 0 <= endPos.y && endPos.y < 8) {
                Piece piece = controller.getPieceAtPos(endPos);
                if (piece && piece.isWhite != isWhite){
                    availableMoves.Add(endPos);
                }
            }
        }
        if (alterCurrentAvailMovesList){
            availMoves = availableMoves;
        }
        return availableMoves;
    }

    public override void PostMove(bool unchanging = false){
        if (!unchanging)
            firstMove = false;
    }

    public override bool CanCaptureOpponentKing()
    {
        int colorMult = isWhite ? 1 : -1; 

        // Only check diagonal capture directions for pawns
        foreach (var direction in captureDirections)
        {
            Vector2Int potentialCapturePos = pos + new Vector2Int(direction.x, direction.y * colorMult);

            if (0 <= potentialCapturePos.x && potentialCapturePos.x < 8 && 0 <= potentialCapturePos.y && potentialCapturePos.y < 8)
            {
                Piece potentialPiece = controller.getPieceAtPos(potentialCapturePos);

                if (potentialPiece != null && potentialPiece.type == PieceType.KING && potentialPiece.isWhite != this.isWhite)
                {
                    return true; // The pawn can capture the opponent's king
                }
            }
        }

        return false; // The pawn cannot capture the opponent's king
    }

    public override void CopyData(Piece source)
    {
        base.CopyData(source);
        if (source is Pawn pawnSource)
        {
            firstMove = pawnSource.firstMove;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    public static Vector2Int[] directions = new Vector2Int[]{
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
    };

    public override List<Vector2Int> CalculateAvailableMoves(bool alterCurrentAvailMovesList = true)
    {
        List<Vector2Int> availableMoves = new List<Vector2Int>();
        foreach (var direction in directions) {
            Vector2Int endPos = pos + direction;
            if (0 <= endPos.x && endPos.x < 8 && 0 <= endPos.y && endPos.y < 8) {
                Piece piece = controller.getPieceAtPos(endPos);
                if (piece == null){
                    availableMoves.Add(endPos);
                }
                else if (piece.isWhite != this.isWhite) {
                    availableMoves.Add(endPos);
                }
            }
        }
        if (alterCurrentAvailMovesList){
            availMoves = availableMoves;
        }
        return availableMoves;
    }

    // Later implement castling rules here (castling will not capture opponent king)
    public override bool CanCaptureOpponentKing()
    {
        foreach (var direction in directions)
        {
            Vector2Int potentialCapturePos = pos + direction;

            if (0 <= potentialCapturePos.x && potentialCapturePos.x < 8 && 0 <= potentialCapturePos.y && potentialCapturePos.y < 8)
            {
                Piece potentialPiece = controller.getPieceAtPos(potentialCapturePos);

                if (potentialPiece != null && potentialPiece.type == PieceType.KING && potentialPiece.isWhite != this.isWhite)
                {
                    return true; // The king can capture the opponent's king (hypothetically)
                }
            }
        }

        return false; // The king cannot capture the opponent's king
    }

    public override void CopyData(Piece source)
    {
        base.CopyData(source);
    }

}
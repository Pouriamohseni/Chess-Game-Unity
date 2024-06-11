using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public static Vector2Int[] directions = new Vector2Int[]{
        new Vector2Int(2, 1),
        new Vector2Int(2, -1),
        new Vector2Int(-2, 1),
        new Vector2Int(-2, -1),
        new Vector2Int(1, 2),
        new Vector2Int(-1, 2),
        new Vector2Int(1, -2),
        new Vector2Int(-1, -2),
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

    public override void CopyData(Piece source)
    {
        base.CopyData(source);
    }
}

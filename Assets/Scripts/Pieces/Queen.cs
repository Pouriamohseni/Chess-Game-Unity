using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
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
            for (int step = 1; step <= Board.ROW_SIZE; step++) {
                Vector2Int endPos = pos + direction * step;
                if (0 <= endPos.x && endPos.x < 8 && 0 <= endPos.y && endPos.y < 8) {
                    // print(1);
                    Piece piece = controller.getPieceAtPos(endPos);
                    if (piece == null){
                        // print(2);
                        availableMoves.Add(endPos);
                    }
                    else if (piece.isWhite != this.isWhite) {
                        // print(3);
                        availableMoves.Add(endPos);
                        break;
                    }
                    else if (piece.isWhite == this.isWhite) {
                        // print(4);
                        break;
                    }
                }
            }
        }
        if (alterCurrentAvailMovesList){
            availMoves = availableMoves;
        }
        return availableMoves;
    }

   
}

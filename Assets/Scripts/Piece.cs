using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public Vector2Int pos;
    public PieceType type;
    public Board board;
    public GameController controller;
    public List<Vector2Int> availMoves;

    [SerializeField] public bool isWhite;

    public abstract List<Vector2Int> CalculateAvailableMoves(bool alterCurrentAvailMovesList = true);

    public void SetData(Vector2Int pos, bool isWhite, Board board, GameController controller) {
        this.isWhite = isWhite;
        this.pos = pos;
        this.board = board;
        this.controller = controller;
        transform.position = board.GetRealCoordsFromGamePos(pos);
    }

    public void MoveTo(Vector2Int endPos)
    {
        // Debug.Log($"Moving {gameObject.name} to {endPos}");
        LeanTween.move(this.gameObject, board.GetRealCoordsFromGamePos(endPos), 0.3f);
        this.pos = endPos;
        // transform.position = board.GetRealCoordsFromGamePos(endPos);
        PostMove();
    }

    public void SecretelyMoveTo(Vector2Int endPos){
        this.pos = endPos;
        PostMove(true);
    }

    public void removeAvailableMove(Vector2Int move){
        availMoves.Remove(move);
    }

    // Overriden by Pawn
    public virtual void PostMove(bool unchanging = false) {
        return;
    }

    

    public virtual bool CanCaptureOpponentKing()
    {
        List<Vector2Int> possibleMoves = availMoves;

        foreach (Vector2Int move in possibleMoves)
        {
            Piece potentialPiece = controller.getPieceAtPos(move);

            if (potentialPiece != null && potentialPiece.type == PieceType.KING && potentialPiece.isWhite != this.isWhite)
            {
                return true;
            }
        }

        return false;
    }


    public Piece CopyToNewPiece()
    {
        Piece newPiece = Instantiate(this);
        newPiece.CopyData(this); 
        return newPiece;
    }

    public virtual void CopyData(Piece source)
    {
        pos = source.pos;
        isWhite = source.isWhite;
    }

    public bool IsValidMove(Vector2Int endPos)
    {
        return availMoves.Contains(endPos);
        //return GetAvailableMoves().Contains(endPos);
    }

    // public virtual bool IsLegal(Vector2Int endPos)
    // {
    //     bool playerColor = this.isWhite;

    //     // Copy the board and simulate move on copied board
    //     Piece[,] copiedBoard = controller.CopyBoardPositions();
    //     copiedBoard[pos.x, pos.y] = null; 
    //     copiedBoard[endPos.x, endPos.y] = this; 

    //     // If the king is in check, the move is not legal
    //     bool isInCheck = controller.IsInCheck(playerColor, copiedBoard);
    //     if (isInCheck)
    //     {
    //         Debug.Log($"Move from {pos} to {endPos} is illegal because it leaves the king in check.");
    //     }
    //     return !isInCheck;
    // }
}
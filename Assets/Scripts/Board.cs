using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public const int ROW_SIZE = 8;

    [SerializeField] private float squareSize;
    [SerializeField] private Vector3 originPos;

    private Piece[,] grid;
    private Piece selectedPiece;
    private GameController controller;

    public Vector3 GetRealCoordsFromGamePos(Vector2Int pos) {
        return new Vector3(originPos.x + pos.x * squareSize, originPos.y, originPos.z + pos.y * squareSize);
    }

    public Vector2Int GetGamePosFromRealCoords(Vector3 pos) {
        int gamePosX = Mathf.RoundToInt((pos.x - originPos.x) / squareSize);
        int gamePosY = Mathf.RoundToInt((pos.z - originPos.z) / squareSize);
        return new Vector2Int(gamePosX, gamePosY);
    }
}
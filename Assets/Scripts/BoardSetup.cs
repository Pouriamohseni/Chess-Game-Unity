using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/StartingSetup")]
public class BoardSetup : ScriptableObject {
    [Serializable]
    public class OccupiedSquare {
        public Vector2Int pos;
        public PieceType pieceType;
        public bool isWhite; // white = true
    }
    
    [SerializeField] public OccupiedSquare[] squares;

    // Returns number of pieces
    public int GetCount(){
        return squares.Length;
    }

    // Returns coordinate of piece at an index
    public Vector2Int GetCoordsAt(int index){
        int x = squares[index].pos.x - 1;
        int y = squares[index].pos.y - 1;
        return new Vector2Int(x, y);
    }

    // Returns the name of a piece at an index
    public string GetPieceNameAt(int index) {
        return squares[index].pieceType.ToString();
    }

    // Returns whether a piece is white at an index
    public bool isWhiteAt(int index) {
        return squares[index].isWhite;
    }
}
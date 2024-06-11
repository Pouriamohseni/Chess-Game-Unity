using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Class to instantiate Piece prefabs
public class PieceFactory : MonoBehaviour
{
    [Serializable]
    private class PiecePrefabSet {
        public GameObject whitePrefab;
        public GameObject blackPrefab;
    }

    [Serializable]
    private class PieceTypePrefabMap {
        public PieceType pieceType;
        public PiecePrefabSet prefabSet;
    }

    [SerializeField] private List<PieceTypePrefabMap> prefabList = new List<PieceTypePrefabMap>();

    private Dictionary<PieceType, PiecePrefabSet> prefabMap = new Dictionary<PieceType, PiecePrefabSet>();

    void Awake() {
        // Convert list to dictionary
        foreach (var item in prefabList) {
            prefabMap[item.pieceType] = item.prefabSet;
        }
    }

    public GameObject CreatePiece(PieceType type, bool isWhite) {
        if (prefabMap.TryGetValue(type, out PiecePrefabSet prefabSet)) {
            return Instantiate(isWhite ? prefabSet.whitePrefab : prefabSet.blackPrefab);
        }

        Debug.LogError($"Piece type {type} not found.");
        return null;
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameController : MonoBehaviour
{

    public GameObject checkmateUI;
    public TextMeshProUGUI winnerText;

    [SerializeField] private BoardSetup startingSetup;
    [SerializeField] private Board board;
    [SerializeField] private Camera gameCamera;

    private PieceFactory pieceFactory; // Creates pieces
    private Highlighter highlighter; // Highlights positions to indicate available moves
    public bool isWhitesTurn; // Keeps track of current turn
    public Piece selectedPiece; // Current piece
    public Piece[,] squares; // Keeps track of piece positions

    public LayerMask chessBoardLayer; // For Raycasting purposes

    void Update() {
        RaycastClick();
    }

    // Detect where click occurs and call handler
    private void RaycastClick() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, chessBoardLayer)) {
                ClickHandler(hit.collider.gameObject, hit.point);
            }
        }
    }


    private void Awake() {
        chessBoardLayer = LayerMask.GetMask("ChessBoard");
        pieceFactory = GetComponent<PieceFactory>();
        highlighter = GetComponent<Highlighter>();
        squares = new Piece[8, 8];
    }

    // Start is called before the first frame update
    void Start() {
        // Initializes board with pieces
        for (int i = 0; i < startingSetup.squares.Length; i++) {
            InitializePiece(startingSetup.squares[i].pos, startingSetup.squares[i].isWhite, startingSetup.squares[i].pieceType);
        }
        isWhitesTurn = true;
        selectedPiece = null;
        NextMove(true);
    }

    private void InitializePiece(Vector2Int pos, bool isWhite, PieceType type) {
        Piece piece = pieceFactory.CreatePiece(type, isWhite).GetComponent<Piece>();
        piece.SetData(pos, isWhite, board, this);
        squares[pos.x, pos.y] = piece;
    }

    public void ClickHandler(GameObject obj, Vector3 pos) {
        Vector2Int clickedPosition = board.GetGamePosFromRealCoords(pos);

        // Returns if mouse clicked off of board
        if (clickedPosition.x < 0 || clickedPosition.x >= 8 || clickedPosition.y < 0 || clickedPosition.y >= 8) return;

        Piece clickedPiece = getPieceAtPos(clickedPosition);

        // If the player clicks on it's own piece, highlight
        // the available move of the selected piece
        if (clickedPiece && clickedPiece.isWhite == isWhitesTurn) {
            selectedPiece = clickedPiece;
            List<Vector2Int> availMoves = selectedPiece.availMoves;
            highlighter.Highlight(availMoves);
        }
        // If the player has selected a piece and
        // clicks on an empty area or the opponent's piece
        else if (selectedPiece && (clickedPiece == null || clickedPiece.isWhite != isWhitesTurn)) {
            AttemptMove(selectedPiece, clickedPosition);
        }
    }

    public void AttemptMove(Piece piece, Vector2Int endPos) {
        if (piece.availMoves.Contains(endPos)) {
            bool isOccupied = squares[endPos.x, endPos.y] != null;
            Vector2Int startPos = piece.pos;
            squares[startPos.x, startPos.y] = null;

            if (isOccupied) {
                // Debug.Log($"Capturing {squares[endPos.x, endPos.y].gameObject.name}");
                Destroy(squares[endPos.x, endPos.y].gameObject);
            }

            piece.MoveTo(endPos);
            squares[endPos.x, endPos.y] = piece; 
            highlighter.Highlight(null);
            NextMove();
        }
    }

    private void NextMove(bool firstMove = false) {
        // Changes turn and resets highlights
        isWhitesTurn = firstMove ? true : !isWhitesTurn;
        selectedPiece = null;

        // Calculates available moves for both players
        calculateAvailableMovesForPlayer(isWhitesTurn);
        calculateAvailableMovesForPlayer(!isWhitesTurn);
        
        // Prevents players from moving to a position which checks their own king
        foreach (Vector2Int position in PiecePositionsForColor(isWhitesTurn, squares)){
            Piece piece = squares[position.x, position.y];
            foreach (Vector2Int move in piece.availMoves.ToList()){
                if (isInCheckAfterMove(piece, move)){
                    // print("Removing move from " + piece.type +  " at position: " + position + " to " + move);
                    piece.removeAvailableMove(move);
                }
            }
        }

        // Ends game on Checkmate or Stalemate
        CheckForEndGame();

        // Moves the camera
        LeanTween.move(gameCamera.gameObject, isWhitesTurn ? new Vector3(0, 15, -9) : new Vector3(0, 15, 9), 0.2f);
        LeanTween.rotate(gameCamera.gameObject, new Vector3(60, isWhitesTurn ? 0 : 180, 0), 0.2f);

        // gameCamera.transform.rotation = Quaternion.Euler(new Vector3(60, isWhitesTurn ? 0 : 180, 0));
        // gameCamera.transform.position = isWhitesTurn ? new Vector3(0, 15, -9) : new Vector3(0, 15, 9);
    }

    public bool isInCheckAfterMove(Piece piece, Vector2Int endPos){
        bool returnValue = false;
        if (!piece.availMoves.Contains(endPos)){
            Debug.LogError("Move is not in list of availableMoves");
        }

        // Saves information before altering
        Vector2Int startPos = new Vector2Int(piece.pos.x, piece.pos.y);
        Piece capturedPiece = squares[endPos.x, endPos.y];

        // Altering
        squares[startPos.x, startPos.y] = null;
        squares[endPos.x, endPos.y] = piece;
        piece.SecretelyMoveTo(endPos);

        // Check if move results in current king being checked
        if (IsInCheck(isWhitesTurn, squares)){
            returnValue = true;
        }

        // Reversing moves
        squares[startPos.x, startPos.y] = piece;
        squares[endPos.x, endPos.y] = capturedPiece;
        piece.SecretelyMoveTo(startPos);

        return returnValue;
    }

    // Calculates all available moves at once
    public void calculateAvailableMovesForPlayer(bool isWhite){
        foreach (Vector2Int position in PiecePositionsForColor(isWhite, squares)){
            getPieceAtPos(position).CalculateAvailableMoves();
        }
    }

    public Piece getPieceAtPos(Vector2Int piecePos) {
        if (piecePos.x < 0 || piecePos.x >= 8 || piecePos.y < 0 || piecePos.y >= 8) {
            return null; 
        }
        return squares[piecePos.x, piecePos.y];
    }

    public List<Vector2Int> PiecePositions() {
        List<Vector2Int> emptyPositions = new List<Vector2Int>();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (squares[x, y] == null)
                { // If the square is empty
                    emptyPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        return emptyPositions;
    }

    private List<Vector2Int> PiecePositionsForColor(bool isWhite, Piece[,] boardState)
    {
        List<Vector2Int> positionsForColor = new List<Vector2Int>();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Piece piece = boardState[x, y];
                if (piece != null && piece.isWhite == isWhite)
                {
                    positionsForColor.Add(new Vector2Int(x, y));
                }
            }
        }

        return positionsForColor;
    }

    private Vector2Int FindKingPosition(bool isWhite, Piece[,] boardState)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Piece piece = boardState[x, y];
                if (piece != null && piece.isWhite == isWhite && piece.type == PieceType.KING)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(-1, -1); // Indicates the king was not found, should never happen
    }

    public bool IsInCheck(bool isWhite, Piece[,] boardState)
    {
        calculateAvailableMovesForPlayer(!isWhitesTurn);
        Vector2Int kingPosition = FindKingPosition(isWhite, boardState);

        bool opponentColor = !isWhite;
        List<Vector2Int> opponentPositions = PiecePositionsForColor(opponentColor, boardState);

        foreach (Vector2Int pos in opponentPositions) {
            Piece opponentPiece = boardState[pos.x, pos.y];
            List<Vector2Int> availableMoves = opponentPiece.availMoves;
            if (availableMoves.Contains(kingPosition)) {
                // Debug.Log($"King is in check by {opponentPiece.gameObject.name} at position {pos}");
                return true; // The king is in check
            }
        }

        return false; // The king is not in check
    }


    public bool IsCheckmateOrStalemate(bool isWhiteTurn) {
        List<Vector2Int> playerPositions = PiecePositionsForColor(isWhiteTurn, squares);
        foreach (Vector2Int position in playerPositions) {
            foreach (Vector2Int move in getPieceAtPos(position).availMoves) {
                return false;
            }
        }

        // No legal moves available for any piece, it's checkmate or stalemate
        return true;
    }

    public void CheckForEndGame()
    {
        bool isCheckmateOrStalemate = IsCheckmateOrStalemate(isWhitesTurn);
        if (isCheckmateOrStalemate)
        {
            bool isInCheck = IsInCheck(isWhitesTurn, squares);
            if (isInCheck)
            {
                // It's checkmate
                string winner = isWhitesTurn ? "Black" : "White";
                ShowCheckmateUI(winner);
            }
            else
            {
                // It's stalemate
                Debug.Log("The game is a stalemate!");
            }
        }
    }

    void ShowCheckmateUI(string winner)
    {
        checkmateUI.SetActive(true);
        winnerText.text = winner + " wins by checkmate!";
    }
}
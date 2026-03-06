using System;
using UnityEngine;

public class ChessGameManager : MonoBehaviour
{
    public LayerMask tileMask;
    public LayerMask pieceMask;

    private ChessPiece selected;
    private PieceColor turn = PieceColor.White;
    private ChessPiece[,] board = new ChessPiece[8, 8];

    void Start()
    {
        // Remplacé FindObjectsOfType obsolète par FindObjectsByType pour supprimer l'avertissement CS0618
        ChessPiece[] pieces = UnityEngine.Object.FindObjectsByType<ChessPiece>(FindObjectsSortMode.None);

        foreach (ChessPiece p in pieces)
        {
            board[p.boardX, p.boardY] = p;
            p.SetPosition(p.boardX, p.boardY);
        }
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f)) return;

        // Clic sur une pièce
        if (((1 << hit.collider.gameObject.layer) & pieceMask) != 0)
        {
            ChessPiece p = hit.collider.GetComponent<ChessPiece>();
            if (p != null && p.color == turn)
            {
                selected = p;
            }
            return;
        }

        // Clic sur une case
        if (((1 << hit.collider.gameObject.layer) & tileMask) != 0 && selected != null)
        {
            Vector3 pos = hit.collider.transform.position;
            int x = Mathf.RoundToInt(pos.x);
            int y = Mathf.RoundToInt(pos.z);

            // Sécurité : vérifier les bornes
            if (x < 0 || x > 7 || y < 0 || y > 7) return;

            // Autoriser case vide ou pièce adverse
            if (board[x, y] == null || board[x, y].color != selected.color)
            {
                if (IsValidMove(selected, x, y))
                {
                    // Capture éventuelle
                    if (board[x, y] != null)
                    {
                        Destroy(board[x, y].gameObject);
                    }

                    // Mettre à jour le tableau
                    board[selected.boardX, selected.boardY] = null;
                    selected.SetPosition(x, y);
                    board[x, y] = selected;

                    // Changer de tour
                    turn = (turn == PieceColor.White) ? PieceColor.Black : PieceColor.White;
                    selected = null;
                }
            }
        }
    }

    private bool IsValidMove(ChessPiece piece, int x, int y)
    {
        // Hors plateau
        if (x < 0 || x > 7 || y < 0 || y > 7) return false;

        // Ne pas rester sur place
        if (x == piece.boardX && y == piece.boardY) return false;

        switch (piece.type)
        {
            case PieceType.Pawn:
                return PawnMove(piece, x, y);

            case PieceType.Rook:
                return RookMove(piece, x, y);

            case PieceType.Knight:
                return KnightMove(piece, x, y);

            case PieceType.Bishop:
                return BishopMove(piece, x, y);

            case PieceType.Queen:
                return QueenMove(piece, x, y);

            case PieceType.King:
                return KingMove(piece, x, y);
        }

        return false;
    }

    private bool PawnMove(ChessPiece piece, int x, int y)
    {
        int dir = (piece.color == PieceColor.White) ? 1 : -1;

        // Avance simple d'une case (doit être vide)
        if (x == piece.boardX && y == piece.boardY + dir && board[x, y] == null)
            return true;

        // Avance de deux cases depuis la position de départ (cases doivent être vides)
        bool isWhiteStart = piece.color == PieceColor.White && piece.boardY == 1;
        bool isBlackStart = piece.color == PieceColor.Black && piece.boardY == 6;
        if (x == piece.boardX && y == piece.boardY + 2 * dir &&
            (isWhiteStart || isBlackStart))
        {
            int midY = piece.boardY + dir;
            if (board[x, midY] == null && board[x, y] == null)
                return true;
        }

        // Capture diagonale (case occupée par une pièce adverse)
        if (Mathf.Abs(x - piece.boardX) == 1 &&
            y == piece.boardY + dir &&
            board[x, y] != null &&
            board[x, y].color != piece.color)
            return true;

        return false;
    }

    private bool RookMove(ChessPiece piece, int x, int y)
    {
        // Doit rester sur la même ligne ou la même colonne
        if (x != piece.boardX && y != piece.boardY) return false;

        // Vérifier qu'aucune pièce ne bloque le chemin
        int stepX = x == piece.boardX ? 0 : (x > piece.boardX ? 1 : -1);
        int stepY = y == piece.boardY ? 0 : (y > piece.boardY ? 1 : -1);

        int cx = piece.boardX + stepX;
        int cy = piece.boardY + stepY;
        while (cx != x || cy != y)
        {
            if (board[cx, cy] != null) return false;
            cx += stepX;
            cy += stepY;
        }

        return true;
    }

    private bool KnightMove(ChessPiece piece, int x, int y)
    {
        int dx = Mathf.Abs(x - piece.boardX);
        int dy = Mathf.Abs(y - piece.boardY);

        // Mouvement en L : 2x1
        return (dx == 1 && dy == 2) || (dx == 2 && dy == 1);
    }

    private bool BishopMove(ChessPiece piece, int x, int y)
    {
        int dx = Mathf.Abs(x - piece.boardX);
        int dy = Mathf.Abs(y - piece.boardY);

        if (dx != dy) return false;

        // Vérifier qu'aucune pièce ne bloque le chemin (diagonale)
        int stepX = x > piece.boardX ? 1 : -1;
        int stepY = y > piece.boardY ? 1 : -1;

        int cx = piece.boardX + stepX;
        int cy = piece.boardY + stepY;
        while (cx != x || cy != y)
        {
            if (board[cx, cy] != null) return false;
            cx += stepX;
            cy += stepY;
        }

        return true;
    }

    private bool QueenMove(ChessPiece piece, int x, int y)
    {
        // Reine = tour ou fou
        return RookMove(piece, x, y) || BishopMove(piece, x, y);
    }

    private bool KingMove(ChessPiece piece, int x, int y)
    {
        int dx = Mathf.Abs(x - piece.boardX);
        int dy = Mathf.Abs(y - piece.boardY);

        // Une case dans n'importe quelle direction
        if (dx <= 1 && dy <= 1)
        {
            // (Plus tard : roque, cases attaquées, etc.)
            return true;
        }

        return false;
    }
}


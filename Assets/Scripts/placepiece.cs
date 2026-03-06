using UnityEngine;

public class PlacePieces : MonoBehaviour
{
    public float tileSize = 1f; // taille d'une case
    public Vector3 boardOrigin = Vector3.zero; // position du coin A1

    void Start()
    {
        ChessPiece[] pieces = FindObjectsByType<ChessPiece>(FindObjectsSortMode.None);

        foreach (ChessPiece piece in pieces)
        {
            Vector3 pos = GetWorldPosition(piece.boardX, piece.boardY);
            piece.transform.position = pos;
        }
    }

    Vector3 GetWorldPosition(int x, int y)
    {
        return boardOrigin + new Vector3(x * tileSize, 0, y * tileSize);
    }
}


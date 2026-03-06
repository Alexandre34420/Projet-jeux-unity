using UnityEngine;

public enum PieceColor { White, Black }
public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King }

public class ChessPiece : MonoBehaviour
{
    public PieceColor color;
    public PieceType type;

    public int boardX;
    public int boardY;

    public void SetPosition(int x, int y)
    {
        boardX = x;
        boardY = y;
        transform.position = new Vector3(x, 0, y);
    }
}

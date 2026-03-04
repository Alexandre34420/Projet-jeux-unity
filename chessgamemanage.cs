using UnityEngine;

public class ChessGameManager : MonoBehaviour
{
    public LayerMask tileMask;
    public LayerMask pieceMask;

    private ChessPiece selected;
    private PieceColor turn = PieceColor.White;
    private ChessPiece[,] board = new ChessPiece[8, 8];

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f)) return;

        if (((1 << hit.collider.gameObject.layer) & pieceMask) != 0)
        {
            ChessPiece p = hit.collider.GetComponent<ChessPiece>();
            if (p.color == turn) selected = p;
            return;
        }

        if (((1 << hit.collider.gameObject.layer) & tileMask) != 0 && selected != null)
        {
            Vector3 pos = hit.collider.transform.position;
            int x = Mathf.RoundToInt(pos.x);
            int y = Mathf.RoundToInt(pos.z);

            if (board[x, y] == null)
            {
                board[selected.boardX, selected.boardY] = null;
                selected.SetPosition(x, y);
                board[x, y] = selected;

                turn = (turn == PieceColor.White) ? PieceColor.Black : PieceColor.White;
                selected = null;
            }
        }
    }
}


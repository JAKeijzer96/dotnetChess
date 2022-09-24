using Core.ChessBoard;

namespace Core.MoveValidators;

public class BishopMoveValidator : IMoveValidator
{
    public bool IsValidMove(Board board, Square from, Square to)
    {
        if (from == to)
            return false;

        if (Math.Abs(from.File - to.File) != Math.Abs(from.Rank - to.Rank))
            return false;

        var xDirection = from.File < to.File ? 1 : -1;
        var yDirection = from.Rank < to.Rank ? 1 : -1;

        for (int x = from.File + xDirection, y = from.Rank + yDirection; x != to.File; x += xDirection, y += yDirection)
        {
            if (board.GetSquare(x, y).Piece != null)
                return false;
        }

        return true;
    }
    
    
}
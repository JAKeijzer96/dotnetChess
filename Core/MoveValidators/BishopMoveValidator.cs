using Core.ChessBoard;

namespace Core.MoveValidators;

public class BishopMoveValidator : MoveValidator
{
    public override bool IsValidMove(Board board, Square from, Square to)
    {
        if (!IsValidDestinationSquare(board, from, to)) return false;
        if (Math.Abs(from.File - to.File) != Math.Abs(from.Rank - to.Rank)) return false;

        int xDirection = from.File < to.File ? 1 : -1;
        int yDirection = from.Rank < to.Rank ? 1 : -1;

        for (int file = from.File + xDirection, rank = from.Rank + yDirection;
             file != to.File;
             file += xDirection, rank += yDirection)
        {
            if (board.GetSquare(file, rank).IsOccupied())
                return false;
        }

        return true;
    }
}
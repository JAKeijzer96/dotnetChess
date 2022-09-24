using Core.ChessBoard;

namespace Core.MoveValidators;

public class BishopMoveValidator : IMoveValidator
{
    public bool IsValidMove(Board board, Square from, Square to)
    {
        if (!IsValidDestinationSquare(board, from, to)) return false;

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

    private static bool IsValidDestinationSquare(Board? board, Square? from, Square? to)
    {
        if (board is null) throw new ArgumentNullException(nameof(board));
        if (from is null) throw new ArgumentNullException(nameof(from));
        if (to is null) throw new ArgumentNullException(nameof(to));

        return from.Piece is not null &&
               from != to &&
               Math.Abs(from.File - to.File) == Math.Abs(from.Rank - to.Rank) &&
               to.Piece?.Color != from.Piece.Color;
    }
}
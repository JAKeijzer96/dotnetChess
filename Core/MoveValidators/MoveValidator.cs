using Core.ChessBoard;

namespace Core.MoveValidators;

public abstract class MoveValidator
{
    public abstract bool IsValidMove(Board board, Square from, Square to);

    protected static bool IsValidDestinationSquare(Board board, Square from, Square to)
    {
        return from.Piece is not null &&
               from != to &&
               to.Piece?.Color != from.Piece.Color;
    }
}
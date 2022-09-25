using Core.ChessBoard;

namespace Core.MoveValidators;

public class KnightMoveValidator : MoveValidator
{
    public override bool IsValidMove(Board board, Square from, Square to)
    {
        return IsValidDestinationSquare(board, from, to);
    }

    protected override bool IsValidDestinationSquare(Board? board, Square? from, Square? to)
    {
        return base.IsValidDestinationSquare(board, from, to) &&
               Math.Abs(from!.File - to!.File) * Math.Abs(from.Rank - to.Rank) == 2;
    }
}
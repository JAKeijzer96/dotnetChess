using Core.ChessBoard;

namespace Core.MoveValidators;

public class KnightMoveValidator : MoveValidator
{
    public override bool IsValidMove(Board board, Square from, Square to)
    {
        return IsValidDestinationSquare(board, from, to) &&
               from.File.DistanceTo(to.File) * from.Rank.DistanceTo(to.Rank) == 2;
    }
}
using Core.ChessBoard;

namespace Core.MoveValidators;

public class KingMoveValidator : MoveValidator
{
    public override bool IsValidMove(Board board, Square from, Square to)
    {
        return IsValidDestinationSquare(board, from, to) &&
               from.File.DistanceTo(to.File) <= 1 &&
               from.Rank.DistanceTo(to.Rank) <= 1;
    }
}
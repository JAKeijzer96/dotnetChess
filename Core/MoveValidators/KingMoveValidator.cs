using Core.ChessBoard;

namespace Core.MoveValidators;

public class KingMoveValidator : MoveValidator
{
    public override bool IsValidMove(Board board, Square from, Square to)
    {
        return IsValidDestinationSquare(board, from, to) &&
               from.File.DistanceTo(to.File) <= 1 &&
               // Math.Abs(from.File - to.File) <= 1 &&
               Math.Abs(from.Rank - to.Rank) <= 1;
    }
}
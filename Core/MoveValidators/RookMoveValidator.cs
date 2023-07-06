using Core.ChessBoard;

namespace Core.MoveValidators;

public class RookMoveValidator : MoveValidator
{
    public override bool IsValidMove(Board board, Square from, Square to)
    {
        if (!IsValidDestinationSquare(board, from, to)) return false;
        if (!(from.File == to.File || from.Rank == to.Rank)) return false;

        if (from.File == to.File && IsValidVerticalMove(from, to, board))
        {
            return true;
        }

        if (from.Rank == to.Rank && IsValidHorizontalMove(from, to, board))
        {
            return true;
        }

        return false;
    }

    private static bool IsValidVerticalMove(Square from, Square to, Board board)
    {
        var yDirection = from.Rank < to.Rank ? 1 : -1;
        for (var rank = from.Rank + yDirection; rank != to.Rank; rank += yDirection)
        {
            if (board.GetSquare(from.File, rank).IsOccupied()) return false;
        }

        return true;
    }

    private static bool IsValidHorizontalMove(Square from, Square to, Board board)
    {
        var xDirection = from.File < to.File ? 1 : -1;
        for (var file = from.File + xDirection; file != to.File; file += xDirection)
        {
            if (board.GetSquare(file, from.Rank).IsOccupied()) return false;
        }

        return true;
    }
}
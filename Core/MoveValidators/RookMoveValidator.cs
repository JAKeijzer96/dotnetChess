using Core.ChessBoard;

namespace Core.MoveValidators;

public class RookMoveValidator : MoveValidator
{
    public override bool IsValidMove(Board board, Square from, Square to)
    {
        if (!IsValidDestinationSquare(board, from, to)) return false;
        if (!(from.File == to.File || from.Rank == to.Rank)) return false;
        
        return (from.File == to.File && IsValidVerticalMove(from, to, board)) ||
               (from.Rank == to.Rank && IsValidHorizontalMove(from, to, board));
    }

    private static bool IsValidVerticalMove(Square from, Square to, Board board)
    {
        var rankDirection = from.Rank < to.Rank ? 1 : -1;
        for (var rank = from.Rank + rankDirection; rank != to.Rank; rank += rankDirection)
        {
            if (board[from.File, rank].IsOccupied())
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsValidHorizontalMove(Square from, Square to, Board board)
    {
        var fileDirection = from.File < to.File ? 1 : -1;
        for (var file = from.File + fileDirection; file != to.File; file += fileDirection)
        {
            if (board[file, from.Rank].IsOccupied())
            {
                return false;
            }
        }

        return true;
    }
}
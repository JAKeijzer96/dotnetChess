using Core.ChessBoard;
using Core.Pieces;
using Core.Shared;

namespace Core.MoveValidators;

public class PawnMoveValidator : MoveValidator
{
    // check if a move from a square to a square is valid for a pawn
    // if the pawn is white, it can only move up the board
    // if the pawn is black, it can only move down the board
    // if the pawn is on its starting square, it can move 1 or 2 squares
    // if the pawn is not on its starting square, it can only move 1 square
    // if the pawn is moving diagonally, it can only move diagonally if it is capturing a piece
    // if the pawn is moving forward, it cannot move to a square that is occupied by a piece

    public override bool IsValidMove(Board board, Square from, Square to)
    {
        if (!IsValidDestinationSquare(board, from, to)) return false;

        var pawn = (Pawn) from.Piece!;
        var direction = pawn.Color == Color.White ? 1 : -1;
        if (from.File == to.File)
        {
            if (from.Rank + direction == to.Rank)
            {
                return !to.IsOccupied();
            }

            if (pawn.IsFirstMove && from.Rank + 2 * direction == to.Rank)
            {
                return !(board.GetSquare(from.File, from.Rank + direction).IsOccupied() || to.IsOccupied());
            }
        }

        if (Math.Abs(from.File - to.File) == 1)
        {
            if (from.Rank + direction == to.Rank)
            {
                // Already checked that if there is a piece on the target square,
                // that it is of the opposite color
                return to.IsOccupied();
            }
        }


        return false;
    }
}
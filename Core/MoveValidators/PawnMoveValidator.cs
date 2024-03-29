﻿using Core.ChessBoard;
using Core.Pieces;

namespace Core.MoveValidators;

public class PawnMoveValidator : MoveValidator
{
    public override bool IsValidMove(Board board, Square from, Square to)
    {
        if (!IsValidDestinationSquare(board, from, to)) return false;

        var pawn = (Pawn) from.Piece!;
        var direction = pawn.IsWhite() ? Direction.Up : Direction.Down;
        if (from.File == to.File)
        {
            if (from.Rank + direction == to.Rank)
            {
                return !to.IsOccupied();
            }

            var isFirstMove = pawn.IsWhite() ? from.Rank == 1 : from.Rank == 6;
            if (from.Rank + 2 * direction == to.Rank && isFirstMove)
            {
                return !(board[from.File, from.Rank + direction].IsOccupied() || to.IsOccupied());
            }

            return false;
        }
        
        if (from.File.DistanceTo(to.File) == 1 && from.Rank + direction == to.Rank)
        {
            // Already checked that if there is a piece on the target square,
            // that it is of the opposite color
            return to.IsOccupied();
        }

        return false;
    }
}
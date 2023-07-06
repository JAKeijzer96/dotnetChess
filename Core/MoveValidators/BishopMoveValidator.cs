﻿using Core.ChessBoard;
using File = Core.ChessBoard.File;

namespace Core.MoveValidators;

public class BishopMoveValidator : MoveValidator
{
    public override bool IsValidMove(Board board, Square from, Square to)
    {
        if (!IsValidDestinationSquare(board, from, to)) return false;
        if (!IsDiagonal(from, to)) return false;

        int fileDirection = from.File < to.File ? 1 : -1;
        int rankDirection = from.Rank < to.Rank ? 1 : -1;

        // TODO: Refactor after refactoring rank
        for (int file = from.File + fileDirection, rank = from.Rank + rankDirection;
             file != to.File;
             file += fileDirection, rank += rankDirection)
        {
            if (board.GetSquare((File) file, rank).IsOccupied())
                return false;
        }

        return true;
    }

    private bool IsDiagonal(Square from, Square to)
    {
        return from.File.DistanceTo(to.File) == Math.Abs(from.Rank - to.Rank);
    }

} 
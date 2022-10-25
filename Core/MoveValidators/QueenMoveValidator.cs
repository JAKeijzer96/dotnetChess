using Core.ChessBoard;

namespace Core.MoveValidators;

public class QueenMoveValidator : MoveValidator
{
    private static readonly BishopMoveValidator BishopMoveValidator = new();
    private static readonly RookMoveValidator RookMoveValidator = new();

    public override bool IsValidMove(Board board, Square from, Square to)
    {
        return BishopMoveValidator.IsValidMove(board, from, to) || RookMoveValidator.IsValidMove(board, from, to);
    }
}
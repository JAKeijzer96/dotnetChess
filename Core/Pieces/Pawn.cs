using Core.ChessBoard;
using Core.MoveValidators;
using Core.Shared;

namespace Core.Pieces;

public class Pawn : Piece
{
    public Pawn(Color color) : base(color, new PawnMoveValidator())
    {
        Name = color == Color.White ? 'P' : 'p';
    }

    public override bool AttacksSquare(Board board, Square from, Square to)
    {
        if (from == to) return false;

        var direction = IsWhite() ? Direction.Up : Direction.Down;

        var isOneDiagonalAhead = from.File.DistanceTo(to.File) == 1
                                 && from.Rank + direction == to.Rank;

        return isOneDiagonalAhead;
    }
}

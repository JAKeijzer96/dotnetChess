using Core.MoveValidators;
using Core.Shared;

namespace Core.Pieces;

public class Pawn : Piece
{
    public Pawn(Color color) : base(color, new PawnMoveValidator())
    {
        Name = color == Color.White ? 'P' : 'p';
    }
}
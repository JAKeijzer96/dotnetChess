using Core.MoveValidators;
using Core.Shared;

namespace Core.Pieces;

public class Bishop : Piece
{
    public Bishop(Color color) : base(color, new BishopMoveValidator())
    {
        Name = color == Color.White ? 'B' : 'b';
    }
}
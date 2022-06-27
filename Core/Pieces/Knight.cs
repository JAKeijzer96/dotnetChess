using Core.MoveValidators;
using Core.Shared;

namespace Core.Pieces;

public class Knight : Piece
{
    public Knight(Color color) : base(color, new KnightMoveValidator())
    {
        Name = color == Color.White ? 'N' : 'n';
    }
}
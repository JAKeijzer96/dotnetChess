using Core.MoveValidators;
using Core.Shared;

namespace Core.Pieces;

public class Rook : Piece
{
    public Rook(Color color) : base(color, new RookMoveValidator())
    {
        Name = color == Color.White ? 'R' : 'r';
    }
}
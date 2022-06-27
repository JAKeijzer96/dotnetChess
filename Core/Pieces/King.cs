using Core.MoveValidators;
using Core.Shared;

namespace Core.Pieces;

public class King : Piece
{
    public King(Color color) : base(color, new KingMoveValidator())
    {
        Name = color == Color.White ? 'K' : 'k';
    }
}
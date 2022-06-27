using Core.MoveValidators;
using Core.Shared;

namespace Core.Pieces;

public class Queen : Piece
{
    public Queen(Color color) : base(color, new QueenMoveValidator())
    {
        Name = color == Color.White ? 'Q' : 'q';
    }
}
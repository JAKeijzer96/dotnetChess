using Core.MoveValidators;
using Core.Shared;

namespace Core.Pieces;

public class Pawn : Piece
{
    public bool IsFirstMove { get; set; }

    public Pawn(Color color, bool isFirstMove = true) : base(color, new PawnMoveValidator())
    {
        Name = color == Color.White ? 'P' : 'p';
        IsFirstMove = isFirstMove;
    }
}
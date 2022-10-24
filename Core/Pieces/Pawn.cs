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
    
    // IsFirstMove should be true only when
    // - pawn square rank == 1 (2nd rank) for white pawns
    // - pawn square rank == 6 (7th rank) for black pawns
    
    // Setting IsFirstMove to true when rank is 1 or 6 regardless of pawn color
    // might not cause bugs (because a white pawn on rank 6 can only move 1 square forward
    // and a black pawn on rank 1 can only move 1 square forward before they hit the edge of the board),
    // but there is no guarantee that it will be bug free

}
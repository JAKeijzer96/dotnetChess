using Core.MoveValidators;
using Core.Shared;

namespace Core.Pieces;

public abstract class Piece
{
    private readonly IMoveValidator _moveValidator;
    public Color Color { get; }
    public char Name { get; protected init; }
    
    protected Piece(Color color, IMoveValidator moveValidator)
    {
        Color = color;
        _moveValidator = moveValidator;
    }

    public override string ToString()
    {
        return Name.ToString();
    }
}
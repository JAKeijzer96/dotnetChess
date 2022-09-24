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

    public static bool operator ==(Piece left, Piece right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Piece x, Piece y)
    {
        return !(x == y);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Piece) obj);
    }

    protected bool Equals(Piece other)
    {
        return Name == other.Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}
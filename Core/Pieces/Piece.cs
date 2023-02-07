using Core.ChessBoard;
using Core.MoveValidators;
using Core.Shared;

namespace Core.Pieces;

public abstract class Piece
{
    private readonly MoveValidator _moveValidator;
    public Color Color { get; }
    public char Name { get; protected init; }

    protected Piece(Color color, MoveValidator moveValidator)
    {
        Color = color;
        _moveValidator = moveValidator;
    }

    public bool IsValidMove(Board board, Square from, Square to)
    {
        return _moveValidator.IsValidMove(board, from, to);
    }

    public bool IsWhite()
    {
        return Color == Color.White;
    }

    public bool IsBlack()
    {
        return Color == Color.Black;
    }

    public override string ToString()
    {
        return Name.ToString();
    }

    public static bool operator ==(Piece? left, Piece? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Piece? left, Piece? right)
    {
        return !(left == right);
    }

    protected bool Equals(Piece? other)
    {
        return other is not null && Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Piece) obj);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}
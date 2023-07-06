using Core.Exceptions;
using Core.Pieces;

namespace Core.ChessBoard;

public class Square
{
    public readonly File File;
    public readonly Rank Rank;
    public Piece? Piece { get; set; }

    public Square(File file, Rank rank, Piece? piece = null)
    {
        File = file;
        Rank = rank;
        Piece = piece;
    }

    public bool IsOccupied()
    {
        return Piece is not null;
    }

    public override string ToString()
    {
        var file = File.ToString();
        var rank = Rank.ToString();
        return file + rank;
    }

    public static bool operator ==(Square? left, Square? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Square? left, Square? right)
    {
        return !(left == right);
    }

    protected bool Equals(Square? other)
    {
        return other is not null && File == other.File && Rank == other.Rank && Piece == other.Piece;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Square) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(File, Rank, Piece);
    }
}
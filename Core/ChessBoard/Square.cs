using Core.Pieces;

namespace Core.ChessBoard;

public class Square
{
    public readonly File File;
    public readonly Rank Rank;
    public Piece? Piece { get; internal set; }

    public Square(File file, Rank rank, Piece? piece = null)
    {
        File = file;
        Rank = rank;
        Piece = piece;
    }

    public bool IsOccupied() => Piece is not null;

    public bool IsEmpty() => Piece is null;

    public bool AttacksSquare(Board board, Square target)
    {
        if (Piece is null)
        {
            return false;
        }

        return Piece.AttacksSquare(board, this, target);
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
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Square)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(File, Rank, Piece);
    }
}
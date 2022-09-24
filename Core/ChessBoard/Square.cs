using Core.Exceptions;
using Core.Pieces;

namespace Core.ChessBoard;

public class Square
{
    public readonly int File;
    public readonly int Rank;
    public Piece? Piece { get; set; }

    public Square(int file, int rank, Piece? piece = null)
    {
        VerifyFileAndRank(file, rank);
        File = file;
        Rank = rank;
        Piece = piece;
    }

    public bool IsOccupied()
    {
        return Piece is not null;
    }

    private static void VerifyFileAndRank(int file, int rank)
    {
        if (file is < 0 or >= Board.BoardSize)
        {
            throw new OutOfBoardException($"File {file} is out of board. Must be between 0 and {Board.BoardSize - 1}");
        }

        if (rank is < 0 or >= Board.BoardSize)
        {
            throw new OutOfBoardException($"Rank {rank} is out of board. Must be between 0 and {Board.BoardSize - 1}");
        }
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
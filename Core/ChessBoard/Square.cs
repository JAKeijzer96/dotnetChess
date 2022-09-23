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
        if (file is < 0 or >= Board.BoardSize)
        {
            throw new OutOfBoardException($"File {file} is out of board. Must be between 0 and {Board.BoardSize - 1}");
        }
        if (rank is < 0 or >= Board.BoardSize)
        {
            throw new OutOfBoardException($"Rank {rank} is out of board. Must be between 0 and {Board.BoardSize - 1}");
        }
        File = file;
        Rank = rank;
        Piece = piece;
    }

    public bool IsOccupied()
    {
        return Piece != null;
    }
    
}
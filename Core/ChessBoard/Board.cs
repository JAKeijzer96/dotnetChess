using Core.Exceptions;
using Core.Pieces;
using Core.Shared;

namespace Core.ChessBoard;

public class Board
{
    private readonly Square[,] _squares;
    public const int BoardSize = 8;

    public Board()
    {
        _squares = new Square[BoardSize, BoardSize];
        SetupBoardWithStartingPosition();
    }

    public Board(string boardFen)
    {
        _squares = new Square[BoardSize, BoardSize];
        SetupBoardWithFen(boardFen);
    }

    private void SetupBoardWithFen(string boardFen)
    {
        if (boardFen == null)
        {
            throw new ArgumentNullException(nameof(boardFen));
        }
        var fenRows = boardFen.Split('/');
        if (fenRows.Length != BoardSize)
        {
            throw new InvalidFenException("Invalid number of ranks in FEN");
        }

        // for (var row = 0; row < BoardSize; row++)
        // {
        //     var fenRow = fenRows[row];
        //     var col = 0;
        //     foreach (var fenChar in fenRow)
        //     {
        //         if (char.IsDigit(fenChar))
        //         {
        //             col += int.Parse(fenChar.ToString());
        //         }
        //         else
        //         {
        //             var piece = PieceFactory.CreatePiece(fenChar);
        //             _squares[row, col] = new Square(piece);
        //             col++;
        //         }
        //     }
        // }
    }

    public Square GetSquare(int file, int rank)
    {
        if (file is < 0 or >= BoardSize)
        {
            throw new OutOfBoardException($"File {file} is out of board. Must be between 0 and {Board.BoardSize - 1}");
        }

        if (rank is < 0 or >= BoardSize)
        {
            throw new OutOfBoardException($"Rank {rank} is out of board. Must be between 0 and {Board.BoardSize - 1}");
        }

        return _squares[file, rank];
    }

    private void SetupBoardWithStartingPosition()
    {
        SetupWhitePiecesStartingPosition();
        SetupWhitePawnsStartingPosition();
        SetupEmptySquaresStartingPosition();
        SetupBlackPawnsStartingPosition();
        SetupBlackPiecesStartingPosition();
    }

    private void SetupWhitePiecesStartingPosition()
    {
        _squares[0, 0] = new Square(0, 0, new Rook(Color.White));
        _squares[1, 0] = new Square(1, 0, new Knight(Color.White));
        _squares[2, 0] = new Square(2, 0, new Bishop(Color.White));
        _squares[3, 0] = new Square(3, 0, new Queen(Color.White));
        _squares[4, 0] = new Square(4, 0, new King(Color.White));
        _squares[5, 0] = new Square(5, 0, new Bishop(Color.White));
        _squares[6, 0] = new Square(6, 0, new Knight(Color.White));
        _squares[7, 0] = new Square(7, 0, new Rook(Color.White));
    }

    private void SetupWhitePawnsStartingPosition()
    {
        for (int file = 0; file < BoardSize; file++)
        {
            _squares[file, 1] = new Square(file, 1, new Pawn(Color.White));
        }
    }

    private void SetupEmptySquaresStartingPosition()
    {
        for (var file = 0; file < BoardSize; file++)
        {
            for (var rank = 2; rank < 6; rank++)
            {
                _squares[file, rank] = new Square(file, rank);
            }
        }
    }

    private void SetupBlackPawnsStartingPosition()
    {
        for (int file = 0; file < BoardSize; file++)
        {
            _squares[file, 6] = new Square(file, 6, new Pawn(Color.Black));
        }
    }

    private void SetupBlackPiecesStartingPosition()
    {
        _squares[0, 7] = new Square(0, 7, new Rook(Color.Black));
        _squares[1, 7] = new Square(1, 7, new Knight(Color.Black));
        _squares[2, 7] = new Square(2, 7, new Bishop(Color.Black));
        _squares[3, 7] = new Square(3, 7, new Queen(Color.Black));
        _squares[4, 7] = new Square(4, 7, new King(Color.Black));
        _squares[5, 7] = new Square(5, 7, new Bishop(Color.Black));
        _squares[6, 7] = new Square(6, 7, new Knight(Color.Black));
        _squares[7, 7] = new Square(7, 7, new Rook(Color.Black));
    }
}
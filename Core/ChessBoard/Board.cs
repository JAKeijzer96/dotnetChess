using System.Text;
using Core.Exceptions;
using Core.Pieces;

namespace Core.ChessBoard;

public class Board
{
    private readonly Square[,] _squares;
    public const int BoardSize = 8;
    private const string DefaultStartingPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

    public Board() : this(DefaultStartingPosition)
    {
    }

    public Board(string boardFen)
    {
        _squares = new Square[BoardSize, BoardSize];
        SetupBoardFromBoardFen(boardFen);
    }

    private void SetupBoardFromBoardFen(string boardFen)
    {
        string[] fenRanks = ValidateAndSplitBoardFen(boardFen);
        int rank = BoardSize - 1;
        int file = 0;
        for (var i = 0; i < BoardSize; i++)
        {
            var fenRank = fenRanks[i];
            foreach (var fenChar in fenRank)
            {
                if (char.IsDigit(fenChar))
                {
                    int emptySquares = fenChar - '0';
                    for (var j = 0; j < emptySquares; j++)
                    {
                        _squares[file, rank] = new Square(file, rank);
                        file++;
                    }
                }
                else
                {
                    AddPieceToBoard(file, rank, fenChar);
                    file++;
                }
            }

            rank--;
            file = 0;
        }
    }

    private void AddPieceToBoard(int file, int rank, char fenChar)
    {
        _squares[file, rank] = new Square(file, rank, PieceFactory.CreatePiece(fenChar));
    }

    public Square GetSquare(int file, int rank)
    {
        VerifyFileAndRankWithinBoard(file, rank);
        return _squares[file, rank];
    }
    
    public Square GetSquare(string squareName)
    {
        if (squareName == null)
        {
            throw new ArgumentNullException(nameof(squareName));
        }

        if (squareName.Length != 2)
        {
            throw new ArgumentException($"Invalid square: {squareName}");
        }

        int file = squareName[0] - 'a';
        int rank = squareName[1] - '1';
        return GetSquare(file, rank);
    }

    public void MovePiece(Square from, Square to)
    {
        to.Piece = from.Piece;
        from.Piece = null;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var rank = BoardSize - 1; rank >= 0; rank--)
        {
            var emptySquares = 0;
            for (var file = 0; file < BoardSize; file++)
            {
                if (_squares[file, rank].IsOccupied())
                {
                    if (emptySquares > 0)
                    {
                        sb.Append(emptySquares);
                        emptySquares = 0;
                    }

                    sb.Append(_squares[file, rank].Piece);
                }
                else
                {
                    emptySquares++;
                }
            }

            if (emptySquares > 0)
            {
                sb.Append(emptySquares);
            }

            sb.Append('/');
        }

        sb.Length--; // Remove last '/'
        return sb.ToString();
    }
    
    #region InternalParameterValidation
    
    private string[] ValidateAndSplitBoardFen(string boardFen)
    {
        if (string.IsNullOrWhiteSpace(boardFen))
        {
            throw new ArgumentNullException(nameof(boardFen));
        }

        if (boardFen.Split(' ').Length > 1)
        {
            throw new ArgumentException("Constructor only accepts board part of FEN string");
        }

        var fenRanks = boardFen.Split('/');
        if (fenRanks.Length != BoardSize)
        {
            throw new InvalidFenException($"Invalid number of ranks in FEN: {boardFen}");
        }

        return fenRanks;
    }
    
    private void VerifyFileAndRankWithinBoard(int file, int rank)
    {
        if (file is < 0 or >= BoardSize)
        {
            throw new OutOfBoardException($"File {file} is out of board. Must be between 0 and {BoardSize - 1}");
        }

        if (rank is < 0 or >= BoardSize)
        {
            throw new OutOfBoardException($"Rank {rank} is out of board. Must be between 0 and {BoardSize - 1}");
        }
    }
    #endregion
}
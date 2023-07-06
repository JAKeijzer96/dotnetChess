using System.Text;
using Core.Exceptions;
using Core.Pieces;

namespace Core.ChessBoard;

public class Board
{
    private readonly Square[,] _squares;
    private const int BoardSize = 8;
    private const string DefaultStartingPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

    public Square this[string squareName] => GetSquare(squareName);

    public Square this[File file, Rank rank] => GetSquare(file, rank);

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
        var rank = Rank.Eighth;
        for (var i = 0; i < BoardSize; i++)
        {
            var fenRank = fenRanks[i];
            AddRankFromFen(rank, fenRank);
            if (rank > Rank.First)
            {
                rank--;
            }
        }
    }

    private void AddRankFromFen(Rank rank, string fenRank)
    {
        var file = File.A;
        foreach (var fenChar in fenRank)
        {
            if (char.IsDigit(fenChar))
            {
                int emptySquares = fenChar - '0';
                for (var j = 0; j < emptySquares; j++)
                {
                    _squares[file, rank] = new Square(file, rank);
                    if (file < File.H)
                    {
                        file++;
                    }
                }
            }
            else
            {
                AddPieceToBoard(file, rank, fenChar);
                if (file < File.H)
                {
                    file++;
                }
            }
        }
    }

    private void AddPieceToBoard(File file, Rank rank, char fenChar)
    {
        _squares[file, rank] = new Square(file, rank, PieceFactory.CreatePiece(fenChar));
    }

    private Square GetSquare(File file, Rank rank)
    {
        return _squares[file, rank];
    }
    
    private Square GetSquare(string squareName)
    {
        if (squareName == null)
        {
            throw new ArgumentNullException(nameof(squareName));
        }

        if (squareName.Length != 2)
        {
            throw new ArgumentException($"Invalid square: {squareName}");
        }

        var file = File.ParseChar(squareName[0]);
        var rank = Rank.ParseChar(squareName[1]);
        return GetSquare(file, rank);
    }

    public void MovePiece(Square from, Square to)
    {
        to.Piece = from.Piece;
        from.Piece = null;
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        var rank = Rank.Eighth;
        while (rank >= Rank.First)
        {
            stringBuilder.Append(RankToString(rank) + '/');
            if (rank == Rank.First)
            {
                break;
            }
            rank--;
        }

        stringBuilder.Length--; // Remove last '/'
        return stringBuilder.ToString();
    }

    private string RankToString(Rank rank)
    {
        var stringBuilder = new StringBuilder();
        var emptySquares = 0;
        var file = File.A;
        while (file <= File.H)
        {
            if (_squares[file, rank].IsOccupied())
            {
                if (emptySquares > 0)
                {
                    stringBuilder.Append(emptySquares);
                    emptySquares = 0;
                }

                stringBuilder.Append(_squares[file, rank].Piece);
            }
            else
            {
                emptySquares++;
            }

            if (file == File.H)
            {
                break;
            }
            file++;
        }

        if (emptySquares > 0)
        {
            stringBuilder.Append(emptySquares);
        }

        return stringBuilder.ToString();
    }
    
    private static string[] ValidateAndSplitBoardFen(string boardFen)
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
}
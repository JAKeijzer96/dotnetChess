using System.Text;
using Core.Exceptions;
using Core.Pieces;
using Core.Shared;

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

    private Board(Square[,] squares)
    {
        _squares = new Square[BoardSize, BoardSize];
        for (File file = File.A; file <= File.H; file++)
        {
            for (Rank rank = Rank.First; rank <= Rank.Eighth; rank++)
            {
                Square original = squares[file, rank];
                _squares[file, rank] = new Square(original.File, original.Rank, original.Piece);
                if (rank == Rank.Eighth) break;
            }
            if (file == File.H) break;
        }
    }

    public Board Clone() => new Board(_squares);

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

    public Square? GetKingSquare(Color color)
    {
        for (File file = File.A; file <= File.H; file++)
        {
            for (Rank rank = Rank.First; rank <= Rank.Eighth; rank++)
            {
                Square square = GetSquare(file, rank);
                if (square.Piece is King king && king.Color == color)
                {
                    return square;
                }

                if (rank == Rank.Eighth) break;
            }
            if (file == File.H) break;
        }
        return null;
    }

    public bool IsKingInCheck(Color color)
    {
        Square? kingSquare = GetKingSquare(color);
        if (kingSquare is null) return false;
        Color attacker = color == Color.White ? Color.Black : Color.White;
        return IsSquareUnderAttack(kingSquare, attacker);
    }

    public bool IsSquareUnderAttack(Square square, Color attacker)
    {
        for (File file = File.A; file <= File.H; file++)
        {
            for (Rank rank = Rank.First; rank <= Rank.Eighth; rank++)
            {
                Square currentSquare = GetSquare(file, rank);
                if (currentSquare.Piece is not null
                    && currentSquare.Piece.Color == attacker
                    && currentSquare.Piece.AttacksSquare(this, currentSquare, square))
                {
                    return true;
                }
                if (rank == Rank.Eighth) break;
            }
            if (file == File.H) break;
        }
        return false;
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
        for (File file = File.A; file <= File.H; file++)
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
            if (file == File.H) break;
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
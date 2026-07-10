using Core.ChessBoard;
using Core.ChessGame;
using Core.Exceptions;
using Core.Pieces;
using Core.Shared;
using System.Text.RegularExpressions;
using File = Core.ChessBoard.File;

namespace Core.Parsers;

public static partial class FenParser
{
    public static Game Parse(string fen)
    {
        string[] splitFen = ValidateAndSplitFen(fen);

        Board board = ParseBoard(splitFen[0]);
        Color turn = ParseTurn(splitFen[1]);
        CastlingAvailability castling = ParseCastling(splitFen[2]);
        ValidateCastlingAgainstBoard(castling, board);
        Square? enPassant = ParseEnPassant(splitFen[3], board);
        int halfMoveCount = ParseHalfMoveCount(splitFen[4]);
        int fullMoveCount = ParseFullMoveCount(splitFen[5]);

        return new Game(board, turn, castling, enPassant, halfMoveCount, fullMoveCount);
    }

    public static string Serialize(Game game)
    {
        var turn = game.Turn == Color.White ? 'w' : 'b';
        var enPassant = game.EnPassant is not null ? game.EnPassant.ToString() : "-";
        return $"{game.Board} {turn} {game.CastlingAvailability} {enPassant} {game.HalfMoveCount} {game.FullMoveCount}";
    }
    
    private static string[] ValidateAndSplitFen(string fen)
    {
        ArgumentNullException.ThrowIfNull(fen);

        string[] fenParts = fen.Split(' ');
        if (fenParts.Length != 6)
        {
            throw new InvalidFenException("FEN string must have 6 parts");
        }

        return fenParts;
    }

    private static Board ParseBoard(string boardFen)
    {
        return new Board(boardFen);
    }

    private static Color ParseTurn(string turnFen) => turnFen switch
    {
        "w" => Color.White,
        "b" => Color.Black,
        _ => throw new InvalidFenException($"Invalid turn: {turnFen}")
    };

    private static CastlingAvailability ParseCastling(string castlingFen)
    {
        return new CastlingAvailability(castlingFen);
    }

    public static void ValidateCastlingAgainstBoard(CastlingAvailability castlingAvailability, Board board)
    {
        if (castlingAvailability.CanNeitherSideCastle()) return;

        if (castlingAvailability.CanWhiteCastleKingside() && board[File.H, Rank.First].Piece is not Rook { IsWhite: true })
            throw new InvalidFenException("FEN castling flag 'K' requires a white rook on h1.");
        if (castlingAvailability.CanWhiteCastleQueenside() && board[File.A, Rank.First].Piece is not Rook { IsWhite: true })
            throw new InvalidFenException("FEN castling flag 'Q' requires a white rook on a1.");
        if (castlingAvailability.CanBlackCastleKingside() && board[File.H, Rank.Eighth].Piece is not Rook { IsBlack: true })
            throw new InvalidFenException("FEN castling flag 'k' requires a black rook on h8.");
        if (castlingAvailability.CanBlackCastleQueenside() && board[File.A, Rank.Eighth].Piece is not Rook { IsBlack: true })
            throw new InvalidFenException("FEN castling flag 'q' requires a black rook on a8.");
    }

    private static Square? ParseEnPassant(string enPassantFen, Board board)
    {
        if (enPassantFen == "-")
        {
            return null;
        }

        if (ValidEnPassantSquareRegex().IsMatch(enPassantFen))
        {
            return board[enPassantFen];
        }

        throw new InvalidFenException($"Invalid en passant square: {enPassantFen}");
    }

    private static int ParseHalfMoveCount(string halfMoveFen)
    {
        var halfMoveCount = int.Parse(halfMoveFen);
        if (halfMoveCount < 0)
        {
            throw new InvalidFenException($"Invalid half move count: {halfMoveFen}");
        }

        return halfMoveCount;
    }

    private static int ParseFullMoveCount(string fullMoveFen)
    {
        var fullMoveCount = int.Parse(fullMoveFen);
        if (fullMoveCount < 1)
        {
            throw new InvalidFenException($"Invalid full move count: {fullMoveFen}");
        }

        return fullMoveCount;
    }

    [GeneratedRegex(@"^[a-h][36]$")]
    private static partial Regex ValidEnPassantSquareRegex();
}
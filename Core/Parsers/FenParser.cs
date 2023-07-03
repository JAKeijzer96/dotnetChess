using System.Text.RegularExpressions;
using Core.ChessBoard;
using Core.ChessGame;
using Core.Exceptions;
using Core.Shared;

namespace Core.Parsers;

public static class FenParser
{
    private static Board _board = null!;
    
    public static Game Parse(string fen)
    {
        string[] splitFen = ValidateAndSplitFen(fen);

        ParseBoard(splitFen[0]);
        Color turn = ParseTurn(splitFen[1]);
        string castling = ParseCastling(splitFen[2]);
        Square? enPassant = ParseEnPassant(splitFen[3]);
        int halfMoveCount = ParseHalfMoveCount(splitFen[4]);
        int fullMoveCount = ParseFullMoveCount(splitFen[5]);

        return new Game(_board, turn, castling, enPassant, halfMoveCount, fullMoveCount);
    }

    public static string Serialize(Game game)
    {
        var turn = game.Turn == Color.White ? 'w' : 'b';
        var enPassant = game.EnPassant is not null ? game.EnPassant.ToString() : "-";
        return $"{game.Board} {turn} {game.Castling} {enPassant} {game.HalfMoveCount} {game.FullMoveCount}";
    }
    
    private static string[] ValidateAndSplitFen(string fen)
    {
        if (fen == null)
        {
            throw new ArgumentNullException(nameof(fen));
        }

        string[] fenParts = fen.Split(' ');
        if (fenParts.Length != 6)
        {
            throw new InvalidFenException("FEN string must have 6 parts");
        }

        return fenParts;
    }

    private static void ParseBoard(string boardFen)
    {
        _board = new Board(boardFen);
    }

    private static Color ParseTurn(string turnFen) => turnFen switch
    {
        "w" => Color.White,
        "b" => Color.Black,
        _ => throw new InvalidFenException($"Invalid turn: {turnFen}")
    };

    private static string ParseCastling(string castlingFen)
    {
        if (Regex.Match(castlingFen, @"^(-|(K?Q?k?q?))$").Success)
        {
            return castlingFen;
        }

        throw new InvalidFenException($"Invalid castling string: {castlingFen}");
    }

    private static Square? ParseEnPassant(string enPassantFen)
    {
        if (enPassantFen == "-")
        {
            return null;
        }

        if (Regex.Match(enPassantFen, @"^[a-h][36]$").Success)
        {
            return _board[enPassantFen];
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
}
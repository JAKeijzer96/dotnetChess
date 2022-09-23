using Core.ChessBoard;
using Core.ChessGame;
using Core.Exceptions;
using Core.Shared;

namespace Core.Parsers;

public class FenParser
{
    public static Game Parse(string fen)
    {
        string[] splitFen = SplitFen(fen);

        Board board = ParseBoard(splitFen[0]);
        Color turn = ParseTurn(splitFen[1]);
        string castling = ParseCastling(splitFen[2]);
        Square? enPassant = ParseEnPassant(splitFen[3]);
        int halfMoveCount = ParseHalfMoveCount(splitFen[4]);
        int fullMoveCount = ParseFullMoveCount(splitFen[5]);

        return new Game(board, turn, castling, enPassant, halfMoveCount, fullMoveCount);
    }


    private static string[] SplitFen(string fen)
    {
        if (fen == null)
        {
            throw new ArgumentNullException();
        }

        string[] fenParts = fen.Split(' ');
        if (fenParts.Length != 6)
        {
            throw new InvalidFenException("FEN string must have 6 parts");
        }

        return fenParts;
    }

    private static Board ParseBoard(string boardFen)
    {
        return BoardFactory.ParsePartialFenNotation(boardFen);
    }

    private static Color ParseTurn(string turnFen)
    {
        return turnFen == "w" ? Color.White : Color.Black;
    }

    private static string ParseCastling(string castlingFen)
    {
        return castlingFen;
    }

    private static Square? ParseEnPassant(string enPassantFen)
    {
        if (enPassantFen == "-")
        {
            return null;
        }

        return null;
    }

    private static int ParseHalfMoveCount(string halfMoveFen)
    {
        return int.Parse(halfMoveFen);
    }

    private static int ParseFullMoveCount(string fullMoveFen)
    {
        return int.Parse(fullMoveFen);
    }
}
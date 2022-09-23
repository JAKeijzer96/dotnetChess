using Core.ChessBoard;
using Core.ChessGame;
using Core.Exceptions;

namespace Core.Parsers;

public class FenParser
{
    string fenTestic = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    public FenGameState Parse(string fen)
    {
        string[] splitFen = SplitFen(fen);

        var board = ParseBoard(splitFen[0]);
        var turn = ParseTurn(splitFen[1]);
        var castling = ParseCastling(splitFen[2]);
        var enPassant = ParseEnPassant(splitFen[3]);
        var halfMoveCount = ParseHalfMoveCount(splitFen[4]);
        var fullMoveCount = ParseFullMoveCount(splitFen[5]);

        return new FenGameState(board, turn, castling, enPassant, halfMoveCount, fullMoveCount);
    }


    private string[] SplitFen(string fen)
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

    private Board ParseBoard(string boardPart)
    {
        return BoardFactory.ParsePartialFenNotation(boardPart);
    }

    private object ParseTurn(string s)
    {
        throw new NotImplementedException();
    }

    private object ParseCastling(string s)
    {
        throw new NotImplementedException();
    }

    private object ParseEnPassant(string s)
    {
        throw new NotImplementedException();
    }

    private object ParseHalfMoveCount(string s)
    {
        throw new NotImplementedException();
    }

    private object ParseFullMoveCount(string s)
    {
        throw new NotImplementedException();
    }
}
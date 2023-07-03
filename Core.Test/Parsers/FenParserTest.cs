using System;
using Core.ChessBoard;
using Core.ChessGame;
using Core.Exceptions;
using Core.Parsers;
using Core.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.Parsers;

[TestClass]
public class FenParserTest
{
    [TestMethod]
    public void Parse_Null_ThrowsArgumentNullException()
    {
        void Act() => FenParser.Parse(null!);

        Assert.ThrowsException<ArgumentNullException>((Action) Act);
    }

    [DataTestMethod]
    [DataRow("rnbqkbnr pppppppp 8 8 8 8 PPPPPPPP RNBQKBNR w KQkq - 0 1")]
    [DataRow("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR")]
    [DataRow("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e0 1")]
    [DataRow("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNRwKQkqc602")]
    public void Parse_FenWitInvalidAmountOfSpaces_ThrowsInvalidFenException(string fen)
    {
        void Act() => FenParser.Parse(fen);

        var exception = Assert.ThrowsException<InvalidFenException>((Action) Act);
        Assert.AreEqual("FEN string must have 6 parts", exception.Message);
    }

    [TestMethod]
    public void Parse_FenWithInvalidTurn_ThrowsInvalidFenException()
    {
        var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR R KQkq - 0 1";

        void Act() => FenParser.Parse(fen);

        var exception = Assert.ThrowsException<InvalidFenException>((Action) Act);
        Assert.AreEqual("Invalid turn: R", exception.Message);
    }

    [DataTestMethod]
    [DataRow("rnbq1rk1/pppp1ppp/5n2/2b1p3/2B1P3/5N2/PPPP1PPP/RNBQ1RK1 w - - 6 5", "-")]
    [DataRow("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "KQkq")]
    [DataRow("rn2kb1N/p3p2p/1p4p1/2pn4/8/3P4/PPPKNq2/1RBQ2R1 b q - 1 14", "q")]
    public void Parse_ParsesValidCastlingStrings(string fen, string castlingString)
    {
        var result = FenParser.Parse(fen);

        Assert.AreEqual(castlingString, result.Castling);
    }

    [DataTestMethod]
    [DataRow("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQX - 0 1", "KQX")]
    [DataRow("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w kqKQ - 0 1", "kqKQ")]
    [DataRow("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KqQk - 0 1", "KqQk")]
    [DataRow("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KqKK - 0 1", "KqKK")]
    public void Parse_FenWithInvalidCastling_ThrowsInvalidFenException(string fen, string castlingString)
    {
        void Act() => FenParser.Parse(fen);

        var exception = Assert.ThrowsException<InvalidFenException>((Action) Act);
        Assert.AreEqual($"Invalid castling string: {castlingString}", exception.Message);
    }

    [TestMethod]
    public void Parse_WithInvalidEnPassantSquare_ThrowsInvalidFenException()
    {
        var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq e2 0 1";

        void Act() => FenParser.Parse(fen);

        var exception = Assert.ThrowsException<InvalidFenException>((Action) Act);
        Assert.AreEqual("Invalid en passant square: e2", exception.Message);
    }

    [TestMethod]
    public void Parse_ParsesValidHalfMoveCount()
    {
        var fen = "4r1k1/3n3p/1pq4b/p4R1p/8/NP2P1P1/PB2K3/2RQ4 b - - 2 36";

        var result = FenParser.Parse(fen);

        Assert.AreEqual(2, result.HalfMoveCount);
    }

    [TestMethod]
    public void Parse_WithInvalidHalfMoveCount_ThrowsInvalidFenException()
    {
        var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - -1 1";

        void Act() => FenParser.Parse(fen);

        var exception = Assert.ThrowsException<InvalidFenException>((Action) Act);
        Assert.AreEqual("Invalid half move count: -1", exception.Message);
    }

    [TestMethod]
    public void Parse_ParsesValidFullMoveCount()
    {
        var fen = "4r1k1/3n3p/1pq4b/p4R1p/8/NP2P1P1/PB2K3/2RQ4 b - - 2 36";

        var result = FenParser.Parse(fen);

        Assert.AreEqual(36, result.FullMoveCount);
    }

    [TestMethod]
    public void Parse_WithInvalidFullMoveCount_ThrowsInvalidFenException()
    {
        var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 0";

        void Act() => FenParser.Parse(fen);

        var exception = Assert.ThrowsException<InvalidFenException>((Action) Act);
        Assert.AreEqual("Invalid full move count: 0", exception.Message);
    }

    [TestMethod]
    public void Serialize_WithNewGame_ReturnsExpectedString()
    {
        var game = new Game();

        var actual = FenParser.Serialize(game);
        
        Assert.AreEqual("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", actual);
    }

    [TestMethod]
    public void Serialize_GameWithAFewMovesPlayed_ReturnsExpectedString()
    {
        var board = new Board("r1b1k2r/ppp1bpp1/4pn1p/1B6/3q3B/2N5/PPP2PPP/R2Q1RK1");
        var game = new Game(board, Color.Black, "kq", null, 1, 11);

        var actual = FenParser.Serialize(game);
        
        Assert.AreEqual("r1b1k2r/ppp1bpp1/4pn1p/1B6/3q3B/2N5/PPP2PPP/R2Q1RK1 b kq - 1 11", actual);
    }
}
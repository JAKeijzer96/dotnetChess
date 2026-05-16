using System;
using System.Threading.Tasks;
using Core.ChessBoard;
using Core.ChessGame;
using Core.Exceptions;
using Core.Parsers;
using Core.Shared;

namespace Core.Test.Parsers;

public class FenParserTest
{
    [Test]
    public async Task Parse_Null_ThrowsArgumentNullException()
    {
        await Assert.That(() => FenParser.Parse(null!))
            .Throws<ArgumentNullException>();
    }

    [Test]
    [Arguments("rnbqkbnr pppppppp 8 8 8 8 PPPPPPPP RNBQKBNR w KQkq - 0 1")]
    [Arguments("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR")]
    [Arguments("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e0 1")]
    [Arguments("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNRwKQkqc602")]
    public async Task Parse_FenWitInvalidAmountOfSpaces_ThrowsInvalidFenException(string fen)
    {
        var exception = await Assert.That(() => FenParser.Parse(fen))
            .Throws<InvalidFenException>();
        await Assert.That(exception.Message).IsEqualTo("FEN string must have 6 parts");
    }

    [Test]
    public async Task Parse_FenWithInvalidTurn_ThrowsInvalidFenException()
    {
        var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR R KQkq - 0 1";

        var exception = await Assert.That(() => FenParser.Parse(fen))
            .Throws<InvalidFenException>();
        await Assert.That(exception.Message).IsEqualTo("Invalid turn: R");
    }

    [Test]
    public async Task Parse_ParsesValidCastlingStrings()
    {
        var result = FenParser.Parse("rn2kb1N/p3p2p/1p4p1/2pn4/8/3P4/PPPKNq2/1RBQ2R1 b q - 1 14");

        await Assert.That(result.CastlingAvailability.ToString()).IsEqualTo("q");
    }

    [Test]
    public async Task Parse_FenWithInvalidCastling_ThrowsInvalidFenException()
    {
        var exception = await Assert.That(() => FenParser.Parse("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQX - 0 1"))
            .Throws<FormatException>();
        await Assert.That(exception.Message).IsEqualTo("Invalid castling format: KQX");
    }

    [Test]
    public async Task Parse_WithInvalidEnPassantSquare_ThrowsInvalidFenException()
    {
        var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq e2 0 1";

        var exception = await Assert.That(() => FenParser.Parse(fen))
            .Throws<InvalidFenException>();
        await Assert.That(exception.Message).IsEqualTo("Invalid en passant square: e2");
    }

    [Test]
    public async Task Parse_ParsesValidHalfMoveCount()
    {
        var fen = "4r1k1/3n3p/1pq4b/p4R1p/8/NP2P1P1/PB2K3/2RQ4 b - - 2 36";

        var result = FenParser.Parse(fen);

        await Assert.That(result.HalfMoveCount).IsEqualTo(2);
    }

    [Test]
    public async Task Parse_WithInvalidHalfMoveCount_ThrowsInvalidFenException()
    {
        var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - -1 1";

        var exception = await Assert.That(() => FenParser.Parse(fen))
            .Throws<InvalidFenException>();
        await Assert.That(exception.Message).IsEqualTo("Invalid half move count: -1");
    }

    [Test]
    public async Task Parse_ParsesValidFullMoveCount()
    {
        var fen = "4r1k1/3n3p/1pq4b/p4R1p/8/NP2P1P1/PB2K3/2RQ4 b - - 2 36";

        var result = FenParser.Parse(fen);

        await Assert.That(result.FullMoveCount).IsEqualTo(36);
    }

    [Test]
    public async Task Parse_WithInvalidFullMoveCount_ThrowsInvalidFenException()
    {
        var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 0";

        var exception = await Assert.That(() => FenParser.Parse(fen))
            .Throws<InvalidFenException>();
        await Assert.That(exception.Message).IsEqualTo("Invalid full move count: 0");
    }

    [Test]
    public async Task Serialize_WithNewGame_ReturnsExpectedString()
    {
        var game = new Game();

        var actual = FenParser.Serialize(game);
        
        await Assert.That(actual).IsEqualTo("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
    }

    [Test]
    public async Task Serialize_GameWithAFewMovesPlayed_ReturnsExpectedString()
    {
        var board = new Board("r1b1k2r/ppp1bpp1/4pn1p/1B6/3q3B/2N5/PPP2PPP/R2Q1RK1");
        var castlingAvailability = new CastlingAvailability("kq");
        var game = new Game(board, Color.Black, castlingAvailability, null, 1, 11);

        var actual = FenParser.Serialize(game);
        
        await Assert.That(actual).IsEqualTo("r1b1k2r/ppp1bpp1/4pn1p/1B6/3q3B/2N5/PPP2PPP/R2Q1RK1 b kq - 1 11");
    }
}
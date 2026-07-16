using System;
using System.Threading.Tasks;
using Core.ChessBoard;
using Core.ChessGame;
using Core.Pieces;
using Core.Shared;

namespace Core.Test.ChessGame;

public class CastlingAvailabilityTest
{
    [Test]
    [Arguments("-")]
    [Arguments("KQkq")]
    [Arguments("q")]
    public async Task CastlingAvailability_WithValidCastlingString_DoesNotThrowException(string castlingString)
    {
        var sut = new CastlingAvailability(castlingString);

        await Assert.That(sut.ToString()).IsEqualTo(castlingString);
    }

    [Test]
    [Arguments("KQX")]
    [Arguments("kqKQ")]
    [Arguments("KqQk")]
    [Arguments(null)]
    public async Task CastlingAvailability_WithInvalidCastlingString_ThrowsFormatException(string? castlingString)
    {
        void Act() => _ = new CastlingAvailability(castlingString!);

        var exception = await Assert.That(Act).Throws<FormatException>();
        await Assert.That(exception!.Message).IsEqualTo($"Invalid castling format: {castlingString}");
    }

    [Test]
    [Arguments("KQkq", Color.White, "kq")]
    [Arguments("Kkq", Color.Black, "K")]
    [Arguments("KQ", Color.White, "-")]
    public async Task AfterCastlingMove_WhiteOrBlack_UpdatesCastlingAvailability(string castling, Color color,
        string expected)
    {
        var sut = new CastlingAvailability(castling);

        var result = sut.AfterCastlingMove(color);

        await Assert.That(result.ToString()).IsEqualTo(expected);
    }

    [Test]
    [Arguments("KQ", 'K', 4, 0, 5, "-")] // Move white king
    [Arguments("KQq", 'R', 0, 0, 1, "Kq")] // Move white a-file rook
    [Arguments("Kq", 'R', 7, 0, 6, "q")] // Move white h-file rook
    [Arguments("Qkq", 'k', 4, 7, 5, "Q")] // Move black king
    [Arguments("q", 'r', 0, 7, 1, "-")] // Move black a-file rook
    [Arguments("Kk", 'r', 7, 7, 6, "K")] // Move black h-file rook
    public async Task AfterRegularMove_MovingRookOrKing_UpdatesCastlingAvailability(string castling, char pieceChar,
        int file, int rank, int toFile, string expectedCastlingValue)
    {
        var sut = new CastlingAvailability(castling);

        var piece = PieceFactory.CreatePiece(pieceChar);
        var result = sut.AfterRegularMove(piece, new Square((File)file, (Rank)rank, piece), new Square((File)toFile, (Rank)rank));

        await Assert.That(result.ToString()).IsEqualTo(expectedCastlingValue);
    }

    [Test]
    public async Task AfterRegularMove_CapturingNonRookPiece_DoesNotChangeCastlingAvailability()
    {
        var sut = new CastlingAvailability("Qkq");

        var movingPiece = PieceFactory.CreatePiece('n');
        var capturedPiece = PieceFactory.CreatePiece('Q');
        var result = sut.AfterRegularMove(
            movingPiece,
            new Square((File)5, (Rank)1, movingPiece),
            new Square((File)7, (Rank)0, capturedPiece));

        await Assert.That(result.ToString()).IsEqualTo("Qkq");
    }

    [Test]
    [Arguments("KQkq", 'n', 5, 1, 7, 0, "Qkq")] // Black captures white h1 rook — strips K
    [Arguments("KQkq", 'n', 5, 1, 0, 0, "Kkq")] // Black captures white a1 rook — strips Q
    [Arguments("KQkq", 'N', 5, 6, 7, 7, "KQq")] // White captures black h8 rook — strips k, leaving KQq
    [Arguments("KQkq", 'N', 5, 6, 0, 7, "KQk")] // White captures black a8 rook — strips q, leaving KQk
    [Arguments("Kkq", 'n', 5, 1, 7, 0, "kq")] // Capture strips the only remaining white right
    public async Task AfterRegularMove_CapturingRookOnHomeSquare_UpdatesCastlingAvailability(string castling,
        char pieceChar, int fromFile, int fromRank, int toFile, int toRank, string expectedCastlingValue)
    {
        var sut = new CastlingAvailability(castling);

        var movingPiece = PieceFactory.CreatePiece(pieceChar);
        char capturedRookChar = char.IsUpper(pieceChar) ? 'r' : 'R'; // attacker captures opposite-color rook
        var capturedRook = PieceFactory.CreatePiece(capturedRookChar);
        var result = sut.AfterRegularMove(
            movingPiece,
            new Square((File)fromFile, (Rank)fromRank, movingPiece),
            new Square((File)toFile, (Rank)toRank, capturedRook));

        await Assert.That(result.ToString()).IsEqualTo(expectedCastlingValue);
    }

    [Test]
    [Arguments('K', "kq")]
    [Arguments('k', "KQ")]
    public async Task AfterRegularMove_KingMoves_DoesNotStripOpposingCastlingRights(char pieceChar, string castling)
    {
        var sut = new CastlingAvailability(castling);

        var king = PieceFactory.CreatePiece(pieceChar);
        var rank = king.IsWhite ? Rank.First : Rank.Eighth;
        var result = sut.AfterRegularMove(king, new Square(File.E, rank, king), new Square(File.F, rank));

        await Assert.That(result.ToString()).IsEqualTo(castling);
    }
}
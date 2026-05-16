using System;
using System.Threading.Tasks;
using Core.ChessBoard;
using Core.ChessGame;
using Core.Pieces;

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
    public async Task CastlingAvailability_WithInvalidCastlingString_ThrowsFormatException(string castlingString)
    {
        void Act() => _ = new CastlingAvailability(castlingString);

        var exception = await Assert.That(Act).Throws<FormatException>();
        await Assert.That(exception!.Message).IsEqualTo($"Invalid castling format: {castlingString}");
    }

    [Test]
    [Arguments("KQkq", true, "kq")]
    [Arguments("Kkq", false, "K")]
    [Arguments("KQ", true, "-")]
    public async Task UpdateAfterCastlingMove_WhiteOrBlack_UpdatesCastlingAvailability(string castling, bool isWhite,
        string expected)
    {
        var sut = new CastlingAvailability(castling);

        sut.UpdateAfterCastlingMove(isWhite);

        await Assert.That(sut.ToString()).IsEqualTo(expected);
    }

    [Test]
    [Arguments("KQ", 'K', 4, 0, "-")] // Move white king
    [Arguments("KQq", 'R', 0, 0, "Kq")] // Move white a-file rook
    [Arguments("Kq", 'R', 7, 0, "q")] // Move white h-file rook
    [Arguments("Qkq", 'k', 4, 7, "Q")] // Move black king
    [Arguments("q", 'r', 0, 7, "-")] // Move black a-file rook
    [Arguments("Kk", 'r', 7, 7, "K")] // Move black h-file rook
    public async Task UpdateAfterRegularMove_MovingRookOrKing_UpdatesCastlingAvailability(string castling, char pieceChar,
        int file, int rank, string expectedCastlingValue)
    {
        var sut = new CastlingAvailability(castling);

        var piece = PieceFactory.CreatePiece(pieceChar);
        sut.UpdateAfterRegularMove(piece, new Square((File) file, (Rank) rank, piece));

        await Assert.That(sut.ToString()).IsEqualTo(expectedCastlingValue);
    }
}
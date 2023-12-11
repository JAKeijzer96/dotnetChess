using System;
using Core.ChessBoard;
using Core.ChessGame;
using Core.Pieces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.ChessGame;

[TestClass]
public class CastlingAvailabilityTest
{
    [DataTestMethod]
    [DataRow("-")]
    [DataRow("KQkq")]
    [DataRow("q")]
    public void CastlingAvailability_WithValidCastlingString_DoesNotThrowException(string castlingString)
    {
        var sut = new CastlingAvailability(castlingString);

        Assert.AreEqual(castlingString, sut.ToString());
    }

    [DataTestMethod]
    [DataRow("KQX")]
    [DataRow("kqKQ")]
    [DataRow("KqQk")]
    [DataRow(null)]
    public void CastlingAvailability_WithInvalidCastlingString_ThrowsFormatException(string castlingString)
    {
        void Act() => _ = new CastlingAvailability(castlingString);

        var exception = Assert.ThrowsException<FormatException>((Action) Act);
        Assert.AreEqual($"Invalid castling format: {castlingString}", exception.Message);
    }

    [DataTestMethod]
    [DataRow("KQkq", true, "kq")]
    [DataRow("Kkq", false, "K")]
    [DataRow("KQ", true, "-")]
    public void UpdateAfterCastlingMove_WhiteOrBlack_UpdatesCastlingAvailability(string castling, bool isWhite,
        string expected)
    {
        var sut = new CastlingAvailability(castling);

        sut.UpdateAfterCastlingMove(isWhite);

        Assert.AreEqual(expected, sut.ToString());
    }

    [DataTestMethod]
    [DataRow("KQ", 'K', 4, 0, "-")] // Move white king
    [DataRow("KQq", 'R', 0, 0, "Kq")] // Move white a-file rook
    [DataRow("Kq", 'R', 7, 0, "q")] // Move white h-file rook
    [DataRow("Qkq", 'k', 4, 7, "Q")] // Move black king
    [DataRow("q", 'r', 0, 7, "-")] // Move black a-file rook
    [DataRow("Kk", 'r', 7, 7, "K")] // Move black h-file rook
    public void UpdateAfterRegularMove_MovingRookOrKing_UpdatesCastlingAvailability(string castling, char pieceChar,
        int file, int rank, string expectedCastlingValue)
    {
        var sut = new CastlingAvailability(castling);

        var piece = PieceFactory.CreatePiece(pieceChar);
        sut.UpdateAfterRegularMove(piece, new Square((File) file, (Rank) rank, piece));

        Assert.AreEqual(expectedCastlingValue, sut.ToString());
    }
}
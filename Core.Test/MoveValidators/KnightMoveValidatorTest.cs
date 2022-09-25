using Core.ChessBoard;
using Core.MoveValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.MoveValidators;

[TestClass]
public class KnightMoveValidatorTest
{
    [DataTestMethod]
    [DataRow("d5", "c3")]
    [DataRow("d5", "b4")]
    [DataRow("d5", "b6")]
    [DataRow("d5", "c7")]
    [DataRow("d5", "e7")]
    [DataRow("d5", "f6")]
    [DataRow("d5", "f4")]
    [DataRow("d5", "e3")]
    public void IsValidMove_ForAll8PossibleKnightMoves_ReturnsTrue(string from, string to)
    {
        var board = new Board("8/8/8/3N4/8/8/8/3k1K2");
        var validator = new KnightMoveValidator();
        var fromSquare = board.GetSquare(from);
        var toSquare = board.GetSquare(to);

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(true, result);
    }

    [DataTestMethod]
    [DataRow("d5", "c6")]
    [DataRow("d5", "b3")]
    [DataRow("d5", "d3")]
    [DataRow("d5", "g8")]
    public void IsValidMove_ForInvalidKnightMove_ReturnsFalse(string from, string to)
    {
        var board = new Board("8/8/8/3N4/8/8/8/3k1K2");
        var validator = new KnightMoveValidator();
        var fromSquare = board.GetSquare(from);
        var toSquare = board.GetSquare(to);

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(false, result);
    }

    [DataTestMethod]
    [DataRow("d5", "c3")]
    [DataRow("d5", "b6")]
    [DataRow("d5", "c7")]
    [DataRow("d5", "e7")]
    public void IsValidMove_WhenJumpingOverPiece_ReturnsTrue(string from, string to)
    {
        var board = new Board("8/8/2q1p3/3N4/2B1P3/8/8/3k1K2");
        var validator = new KnightMoveValidator();
        var fromSquare = board.GetSquare(from);
        var toSquare = board.GetSquare(to);

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(true, result);
    }
}
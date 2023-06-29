using Core.ChessBoard;
using Core.MoveValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.MoveValidators;

[TestClass]
public class KnightMoveValidatorTest
{
    [DataTestMethod]
    [DataRow("d5", "c3")] // 1 left 2 down
    [DataRow("d5", "b4")] // 2 left 1 down
    [DataRow("d5", "b6")] // 2 left 1 up
    [DataRow("d5", "c7")] // 1 left 2 up
    [DataRow("d5", "e7")] // 1 right 2 up
    [DataRow("d5", "f6")] // 2 right 1 up
    [DataRow("d5", "f4")] // 2 right 1 down
    [DataRow("d5", "e3")] // 1 right 2 down
    public void IsValidMove_ForAll8PossibleKnightMoves_ReturnsTrue(string from, string to)
    {
        var board = new Board("8/8/2q1p3/3N4/2B1P3/8/8/3k1K2");
        var validator = new KnightMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

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
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(false, result);
    }
}
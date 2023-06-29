using Core.ChessBoard;
using Core.MoveValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.MoveValidators;

[TestClass]
public class KingMoveValidatorTest
{
    [DataTestMethod]
    [DataRow("f6", "e6")] // Left
    [DataRow("f6", "g6")] // Right
    [DataRow("f6", "f7")] // Up
    [DataRow("f6", "f5")] // Down
    [DataRow("f6", "e5")] // Down left
    [DataRow("f6", "e7")] // Up left
    [DataRow("f6", "g7")] // Up right
    [DataRow("f6", "g5")] // Down right
    public void IsValidMove_ForOneSquareMove_ReturnsTrue(string from, string to)
    {
        var board = new Board("8/8/PK3k2/8/8/8/8/8");
        var validator = new KingMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(true, result);
    }

    [DataTestMethod]
    [DataRow("f6", "d4")]
    [DataRow("f6", "d7")]
    [DataRow("f6", "f8")]
    [DataRow("f6", "h5")]
    [DataRow("f6", "h4")]
    public void IsValidMove_ForMultipleSquaresAtOnce_ReturnsFalse(string from, string to)
    {
        var board = new Board("8/8/PK3k2/8/8/8/8/8");
        var validator = new KingMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(false, result);
    }
}
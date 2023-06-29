using Core.ChessBoard;
using Core.MoveValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.MoveValidators;

[TestClass]
public class BishopMoveValidatorTest
{
    [DataTestMethod]
    [DataRow("e4", "b1")] // Down left
    [DataRow("e4", "a8")] // Up left
    [DataRow("e4", "h7")] // Down right
    [DataRow("e4", "h1")] // Up right
    public void IsValidMove_ForDiagonalMove_ReturnsTrue(string from, string to)
    {
        var board = new Board("8/8/8/8/4B3/8/4P3/k3K3");
        var validator = new BishopMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsTrue(result);
    }

    [DataTestMethod]
    [DataRow("e4", "e8")] // Up
    [DataRow("e4", "e1")] // Down
    [DataRow("e4", "a4")] // Left
    [DataRow("e4", "h4")] // Right
    public void IsValidMove_ForMoveThatIsNotDiagonal_ReturnsFalse(string from, string to)
    {
        var board = new Board("8/8/8/8/4B3/8/4P3/k3K3");
        var validator = new BishopMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(false, result);
    }

    [DataTestMethod]
    [DataRow("d6", "f8")] // Blocked by opposite color
    [DataRow("d6", "h2")] // Blocked by same color
    public void IsValidMove_WhenMoveIsBlocked_ReturnsFalse(string from, string to)
    {
        var board = new Board("1N6/4P3/3b4/8/8/r5k1/8/5K2");
        var validator = new BishopMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(false, result);
    }
}
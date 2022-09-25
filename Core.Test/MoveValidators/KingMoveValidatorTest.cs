using Core.ChessBoard;
using Core.MoveValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.MoveValidators;

[TestClass]
public class KingMoveValidatorTest
{
    [DataTestMethod]
    [DataRow("e4", "b1")]
    [DataRow("e4", "a8")]
    [DataRow("e4", "h7")]
    [DataRow("e4", "h1")]
    public void IsValidMove_ForDiagonalMove_ReturnsTrue(string from, string to)
    {
        var board = new Board("8/8/8/8/4B3/8/4P3/k3K3");
        var validator = new KingMoveValidator();
        var fromSquare = board.GetSquare(from);
        var toSquare = board.GetSquare(to);

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(false, result);
    }

    [DataTestMethod]
    [DataRow("e4", "e8")]
    [DataRow("e4", "e1")]
    [DataRow("e4", "a4")]
    [DataRow("e4", "h4")]
    public void IsValidMove_ForHorizontalMove_ReturnsTrue(string from, string to)
    {
        var board = new Board("8/8/8/8/4B3/8/4P3/k3K3");
        var validator = new KingMoveValidator();
        var fromSquare = board.GetSquare(from);
        var toSquare = board.GetSquare(to);

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(false, result);
    }

    [DataTestMethod]
    [DataRow("e4", "e8")]
    [DataRow("e4", "e1")]
    [DataRow("e4", "a4")]
    [DataRow("e4", "h4")]
    public void IsValidMove_ForVertical_ReturnsTrue(string from, string to)
    {
        var board = new Board("8/8/8/8/4B3/8/4P3/k3K3");
        var validator = new KingMoveValidator();
        var fromSquare = board.GetSquare(from);
        var toSquare = board.GetSquare(to);

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(false, result);
    }
}
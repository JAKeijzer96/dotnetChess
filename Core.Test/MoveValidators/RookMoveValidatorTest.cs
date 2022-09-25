using Core.ChessBoard;
using Core.MoveValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.MoveValidators;

[TestClass]
public class RookMoveValidatorTest
{
    [DataTestMethod]
    [DataRow("c5", "a5")]
    [DataRow("c5", "f5")]
    public void IsValidMove_ForHorizontalMove_ReturnsTrue(string from, string to)
    {
        var board = new Board("8/2b5/8/2R5/8/8/k1K5/8");
        var validator = new RookMoveValidator();
        var fromSquare = board.GetSquare(from);
        var toSquare = board.GetSquare(to);

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(true, result);
    }

    [DataTestMethod]
    [DataRow("c5", "c6")]
    [DataRow("c5", "c3")]
    public void IsValidMove_ForVerticalMove_ReturnsTrue(string from, string to)
    {
        var board = new Board("8/2b5/8/2R5/8/8/k1K5/8");
        var validator = new RookMoveValidator();
        var fromSquare = board.GetSquare(from);
        var toSquare = board.GetSquare(to);

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(true, result);
    }
    
    [DataTestMethod]
    [DataRow("c5", "a7")]
    [DataRow("c5", "b7")]
    [DataRow("c5", "g1")]
    [DataRow("c5", "a2")]
    public void IsValidMove_ForMoveThatIsNotHorizontalOrVertical_ReturnsFalse(string from, string to)
    {
        var board = new Board("8/2b5/8/2R5/8/8/k1K5/8");
        var validator = new RookMoveValidator();
        var fromSquare = board.GetSquare(from);
        var toSquare = board.GetSquare(to);

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void IsValidMove_WhenMoveIsBlockedByPieceOfOppositeColor_ReturnsFalse()
    {
        var board = new Board("8/2b5/8/2R5/8/8/k1K5/8");
        var validator = new RookMoveValidator();
        var fromSquare = board.GetSquare("c5");
        var toSquare = board.GetSquare("c8");

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void IsValidMove_WhenMoveIsBlockedByPieceOfSameColor_ReturnsFalse()
    {
        var board = new Board("8/2b5/8/2R5/8/8/k1K5/8");
        var validator = new RookMoveValidator();
        var fromSquare = board.GetSquare("c5");
        var toSquare = board.GetSquare("c1");

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(false, result);
    }
}
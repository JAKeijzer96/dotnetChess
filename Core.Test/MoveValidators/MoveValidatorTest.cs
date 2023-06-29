using System;
using Core.ChessBoard;
using Core.MoveValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.MoveValidators;

[TestClass]
public class MoveValidatorTest
{
    [TestMethod]
    public void IsValidMove_WhenPieceIsNull_ReturnsFalse()
    {
        var board = new Board("8/8/8/8/4B3/8/4P3/k3K3");
        MoveValidator validator = new KingMoveValidator();
        var fromSquare = board["d5"];
        var toSquare = board["c6"];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void IsValidMove_ToSameSquare_ReturnsFalse()
    {
        var board = new Board("8/8/8/8/4B3/8/4P3/k3K3");
        MoveValidator validator = new KingMoveValidator();
        var fromSquare = board["e1"];
        var toSquare = board["e1"];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void IsValidMove_WhenDestinationSquareHasPieceOfOppositeColor_ReturnsTrue()
    {
        var board = new Board("1N6/4P3/3b4/8/8/r5k1/8/5K2");
        var validator = new BishopMoveValidator();
        var fromSquare = board["d6"];
        var toSquare = board["b8"];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void IsValidMove_WhenDestinationSquareHasPieceOfSameColor_ReturnsFalse()
    {
        var board = new Board("1N6/4P3/3b4/8/8/r5k1/8/5K2");
        MoveValidator validator = new BishopMoveValidator();
        var fromSquare = board["d6"];
        var toSquare = board["a3"];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.AreEqual(false, result);
    }
}
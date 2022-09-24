using System;
using Core.ChessBoard;
using Core.MoveValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.MoveValidators;

[TestClass]
public class BishopMoveValidatorTest
{
    [TestMethod]
    public void IsValidMove_WhenBoardIsNull_ThrowsArgumentNullException()
    {
        Board? board = null;
        var validator = new BishopMoveValidator();
        var fromSquare = new Square(0, 0);
        var toSquare = new Square(1, 1);

        void Act()
        {
            validator.IsValidMove(board!, fromSquare, toSquare);
        }

        var exception = Assert.ThrowsException<ArgumentNullException>((Action) Act);
        Assert.AreEqual("Value cannot be null. (Parameter 'board')", exception.Message);
    }
    
    [TestMethod]
    public void IsValidMove_WhenStartingSquareIsNull_ReturnsFalse()
    {
        var board = new Board();
        var validator = new BishopMoveValidator();
        Square? fromSquare = null;
        var toSquare = new Square(1, 1);

        void Act()
        {
            validator.IsValidMove(board, fromSquare!, toSquare);
        }

        var exception = Assert.ThrowsException<ArgumentNullException>((Action) Act);
        Assert.AreEqual("Value cannot be null. (Parameter 'from')", exception.Message);
    }
    
    [TestMethod]
    public void IsValidMove_WhenDestinationSquareIsNull_ReturnsFalse()
    {
        var board = new Board();
        var validator = new BishopMoveValidator();
        var fromSquare = new Square(0, 0);
        Square? toSquare = null;

        void Act()
        {
            validator.IsValidMove(board, fromSquare, toSquare!);
        }

        var exception = Assert.ThrowsException<ArgumentNullException>((Action) Act);
        Assert.AreEqual("Value cannot be null. (Parameter 'to')", exception.Message);
    }
    
    [TestMethod]
    public void IsValidMove_WhenPieceIsNull_ReturnsFalse()
    {
        var board = new Board("8/8/8/8/4B3/8/4P3/k3K3");
        var validator = new BishopMoveValidator();
        var fromSquare = board.GetSquare("d5");
        var toSquare = board.GetSquare("c6");
        
        var result = validator.IsValidMove(board, fromSquare, toSquare);
        
        Assert.AreEqual(false, result);
    }
    
    [TestMethod]
    public void IsValidMove_ToSameSquare_ReturnsFalse()
    {
        var board = new Board("8/8/8/8/4B3/8/4P3/k3K3");
        var validator = new BishopMoveValidator();
        var fromSquare = board.GetSquare("e4");
        var toSquare = board.GetSquare("e4");
        
        var result = validator.IsValidMove(board, fromSquare, toSquare);
        
        Assert.AreEqual(false, result);
    }
    
    [DataTestMethod]
    [DataRow("e4", "b1")]
    [DataRow("e4", "a8")]
    [DataRow("e4", "h7")]
    [DataRow("e4", "h1")]
    public void IsValidMove_ForDiagonalMove_ReturnsTrue(string from, string to)
    {
        var board = new Board("8/8/8/8/4B3/8/4P3/k3K3");
        var validator = new BishopMoveValidator();
        var fromSquare = board.GetSquare(from);
        var toSquare = board.GetSquare(to);
        
        var result = validator.IsValidMove(board, fromSquare, toSquare);
        
        Assert.IsTrue(result);
    }

    [DataTestMethod]
    [DataRow("e4", "e8")]
    [DataRow("e4", "e1")]
    [DataRow("e4", "a4")]
    [DataRow("e4", "h4")]
    public void IsValidMove_ForMoveThatIsNotDiagonal_ReturnsFalse(string from, string to)
    {
        var board = new Board("8/8/8/8/4B3/8/4P3/k3K3");
        var validator = new BishopMoveValidator();
        var fromSquare = board.GetSquare(from);
        var toSquare = board.GetSquare(to);
        
        var result = validator.IsValidMove(board, fromSquare, toSquare);
        
        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void IsValidMove_WhenMoveIsBlockedByPieceOfOppositeColor_ReturnsFalse()
    {
        var board = new Board("1N6/4P3/3b4/8/8/r5k1/8/5K2");
        var validator = new BishopMoveValidator();
        var fromSquare = board.GetSquare("d6");
        var toSquare = board.GetSquare("f8");
        
        var result = validator.IsValidMove(board, fromSquare, toSquare);
        
        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void IsValidMove_WhenMoveIsBlockedByPieceOfSameColor_ReturnsFalse()
    {
        var board = new Board("1N6/4P3/3b4/8/8/r5k1/8/5K2");
        var validator = new BishopMoveValidator();
        var fromSquare = board.GetSquare("d6");
        var toSquare = board.GetSquare("h2");
        
        var result = validator.IsValidMove(board, fromSquare, toSquare);
        
        Assert.AreEqual(false, result);
    }
    
    [TestMethod]
    public void IsValidMove_WhenDestinationSquareHasPieceOfOppositeColor_ReturnsTrue()
    {
        var board = new Board("1N6/4P3/3b4/8/8/r5k1/8/5K2");
        var validator = new BishopMoveValidator();
        var fromSquare = board.GetSquare("d6");
        var toSquare = board.GetSquare("b8");
        
        var result = validator.IsValidMove(board, fromSquare, toSquare);
        
        Assert.AreEqual(true, result);
    }
    
    [TestMethod]
    public void IsValidMove_WhenDestinationSquareHasPieceOfSameColor_ReturnsFalse()
    {
        var board = new Board("1N6/4P3/3b4/8/8/r5k1/8/5K2");
        var validator = new BishopMoveValidator();
        var fromSquare = board.GetSquare("d6");
        var toSquare = board.GetSquare("a3");
        
        var result = validator.IsValidMove(board, fromSquare, toSquare);
        
        Assert.AreEqual(false, result);
    }

}
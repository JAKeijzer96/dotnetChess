using Core.ChessBoard;
using Core.MoveValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.MoveValidators;

[TestClass]
public class PawnMoveValidatorTest
{
    [TestMethod]
    public void IsValidMove_OneForwardOnFirstMove_IsTrue()
    {
        // Arrange
        // 8/8/8/2q5/4q3/2q2qP1/3PPP2/5K1k w - - 0 1
        
        var board = new Board("8/8/8/2q5/4q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board.GetSquare("d2");
        var toSquare = board.GetSquare("d3");

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsTrue(result);
    }
    
    [TestMethod]
    public void IsValidMove_TwoForwardOnFirstMove_IsTrue()
    {
        var board = new Board("8/8/8/2q5/4q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board.GetSquare("d2");
        var toSquare = board.GetSquare("d4");

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsTrue(result);
    }
    
    [TestMethod]
    public void IsValidMove_OneForwardWithBlockingPieceInFront_IsFalse()
    {
        var board = new Board("8/8/8/2q5/4q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board.GetSquare("f2");
        var toSquare = board.GetSquare("f3");

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsValidMove_TwoForwardWithBlockingPieceOnDestinationSquare_IsFalse()
    {
        var board = new Board("8/8/8/2q5/4q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board.GetSquare("e2");
        var toSquare = board.GetSquare("e4");

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void IsValidMove_TwoForwardWithBlockingPieceBetweenFromAndTo_IsFalse()
    {
        var board = new Board("8/8/8/2q5/4q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board.GetSquare("f2");
        var toSquare = board.GetSquare("f4");

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void IsValidMove_TwoForwardWithoutItBeingFirstMove_IsFalse()
    {
        var board = new Board("8/8/8/2q5/4q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board.GetSquare("g3");
        var toSquare = board.GetSquare("g5");

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void IsValidMove_OneBackward_IsFalse()
    {
        var board = new Board("8/8/8/2q5/4q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board.GetSquare("d2");
        var toSquare = board.GetSquare("d1");

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsFalse(result);
    }
    
    [DataTestMethod]
    [DataRow("d2", "c3")]
    [DataRow("e2", "f3")]
    public void IsValidMove_CapturingOtherColorPieceOneSquareDiagonally_IsTrue(string from, string to)
    {
        var board = new Board("8/8/8/2q5/4q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board.GetSquare(from);
        var toSquare = board.GetSquare(to);

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsTrue(result);
    }
    
    [TestMethod]
    public void IsValidMove_CapturingSameColorPieceOneSquareDiagonally_IsFalse()
    {
        var board = new Board("8/8/8/2q5/4q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board.GetSquare("f2");
        var toSquare = board.GetSquare("g3");

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void IsValidMove_CapturingOtherColorPieceMultipleSquaresDiagonally_IsFalse()
    {
        var board = new Board("8/8/8/2q5/4q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board.GetSquare("f2");
        var toSquare = board.GetSquare("c5");

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void IsValidMove_MovingDiagonallyToEmptySquare_IsFalse()
    {
        var board = new Board("8/8/8/2q5/4q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board.GetSquare("e3");
        var toSquare = board.GetSquare("d3");

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsFalse(result);
    }
    
    }
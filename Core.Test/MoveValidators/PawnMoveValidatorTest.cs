using Core.ChessBoard;
using Core.MoveValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.MoveValidators;

[TestClass]
public class PawnMoveValidatorTest
{
    [DataTestMethod]
    [DataRow("d2", "d3")]
    [DataRow("d7", "d6")]
    public void IsValidMove_OneForwardOnFirstMove_IsTrue(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsTrue(result);
    }

    [DataTestMethod]
    [DataRow("d2", "d4")]
    [DataRow("d7", "d5")]
    public void IsValidMove_TwoForwardOnFirstMove_IsTrue(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsTrue(result);
    }

    [DataTestMethod]
    [DataRow("f2", "f3")]
    [DataRow("f7", "f6")]
    public void IsValidMove_OneForwardWithBlockingPieceInFront_IsFalse(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsFalse(result);
    }

    [DataTestMethod]
    [DataRow("e2", "e4")]
    [DataRow("e7", "e5")]
    public void IsValidMove_TwoForwardWithBlockingPieceOnDestinationSquare_IsFalse(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsFalse(result);
    }

    [DataTestMethod]
    [DataRow("f2", "f4")]
    [DataRow("f7", "f5")]
    public void IsValidMove_TwoForwardWithBlockingPieceBetweenFromAndTo_IsFalse(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsFalse(result);
    }

    [DataTestMethod]
    [DataRow("g3", "g5")]
    [DataRow("g6", "g4")]
    public void IsValidMove_TwoForwardWithoutItBeingFirstMove_IsFalse(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsFalse(result);
    }

    [DataTestMethod]
    [DataRow("d2", "d1")]
    [DataRow("d7", "d8")]
    public void IsValidMove_OneBackward_IsFalse(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsFalse(result);
    }

    [DataTestMethod]
    [DataRow("d2", "c3")]
    [DataRow("e2", "f3")]
    [DataRow("d7", "c6")]
    [DataRow("e7", "f6")]
    public void IsValidMove_CapturingOtherColorPieceOneSquareDiagonally_IsTrue(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsTrue(result);
    }

    [DataTestMethod]
    [DataRow("f2", "g3")]
    [DataRow("f7", "g6")]
    public void IsValidMove_CapturingSameColorPieceOneSquareDiagonally_IsFalse(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsFalse(result);
    }

    [DataTestMethod]
    [DataRow("f2", "c5")]
    [DataRow("f7", "c4")]
    public void IsValidMove_CapturingOtherColorPieceMultipleSquaresDiagonally_IsFalse(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsFalse(result);
    }

    [DataTestMethod]
    [DataRow("e2", "d3")]
    [DataRow("e7", "d6")]
    public void IsValidMove_MovingDiagonallyToEmptySquare_IsFalse(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        Assert.IsFalse(result);
    }
}
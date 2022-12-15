using Core.ChessBoard;
using Core.ChessGame;
using Core.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.ChessGame;

[TestClass]
public class GameTest
{
    [DataTestMethod]
    [DataRow(Color.White, "e7", "e5")]
    [DataRow(Color.Black, "e2", "e4")]
    public void MakeMove_MovingOpponentsPiece_ReturnsFalse(Color turn, string from, string to)
    {
        // Arrange
        var sut = new Game(new Board(), turn, "KQkq", null, 0, 1);

        // Act
        var result = sut.MakeMove(from, to);

        // Assert
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void MakeMove_WhenPieceIsNull_ReturnsFalse()
    {
        // Arrange
        var sut = new Game(new Board(), Color.White, "KQkq", null, 0, 1);

        // Act
        var result = sut.MakeMove("a4", "a5");

        // Assert
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void MakeMove_WithInvalidMove_ReturnsFalse()
    {
        // Arrange
        var sut = new Game(new Board(), Color.White, "KQkq", null, 0, 1);

        // Act
        var result = sut.MakeMove("e2", "b4");

        // Assert
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void MakeMove_WithValidMoveToEmptySquare_MovesPieceToCorrectSquare()
    {
        // Arrange
        var sut = new Game(new Board(), Color.White, "KQkq", null, 0, 1);

        // Act
        var result = sut.MakeMove("e2", "e4");

        // Assert
        var e2Piece = sut.Board.GetSquare("e2").Piece;
        var e4Piece = sut.Board.GetSquare("e4").Piece;
        Assert.IsTrue(result);
        Assert.IsNull(e2Piece);
        Assert.AreEqual('P', e4Piece!.Name);
    }
    
    [TestMethod]
    public void MakeMove_WithValidMoveToOccupiedSquare_CapturesPieceOnThatSquare()
    {
        // Arrange
        var sut = new Game(new Board("rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR"), Color.White, "KQkq", null, 0, 2);

        // Act
        var result = sut.MakeMove("e4", "d5");

        // Assert
        var e4Piece = sut.Board.GetSquare("e4").Piece;
        var d5Piece = sut.Board.GetSquare("d5").Piece;
        Assert.IsTrue(result);
        Assert.IsNull(e4Piece);
        Assert.AreEqual('P', d5Piece!.Name);
    }
}
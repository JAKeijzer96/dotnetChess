using System;
using Core.ChessBoard;
using Core.Exceptions;
using Core.Pieces;
using Core.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.ChessBoard;

[TestClass]
public class BoardTest
{
    [TestMethod]
    public void Board_WithStartingPosition_PutsWhiteKingOnE1()
    {
        var board = new Board();
        var expectedPiece = new King(Color.White);

        var actualPiece = board.GetSquare(4, 0).Piece;
        
        Assert.IsNotNull(actualPiece);
        Assert.AreEqual(expectedPiece.Color, actualPiece.Color);
        Assert.AreEqual(expectedPiece.Name, actualPiece.Name);
    }
    
    [TestMethod]
    public void Board_WithStartingPosition_PutsBlackQueenOnD8()
    {
        var board = new Board();
        var expectedPiece = new Queen(Color.Black);
        
        var actualPiece = board.GetSquare(3, 7).Piece;
        
        Assert.IsNotNull(actualPiece);
        Assert.AreEqual(expectedPiece.Color, actualPiece.Color);
        Assert.AreEqual(expectedPiece.Name, actualPiece.Name);
    }

    [TestMethod]
    public void ToString_OfBoardWithStartingPosition_ReturnsCorrectString()
    {
        var boardFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

        var board = new Board(boardFen);
        
        Assert.AreEqual(boardFen, board.ToString());
    }

    [DataTestMethod]
    [DataRow(0, 0)]
    [DataRow(1, 7)]
    public void GetSquare_WithValidCoordinates_ReturnsSquare(int file, int rank)
    {
        var board = new Board();
        
        var result = board.GetSquare(file, rank);
        
        Assert.AreEqual(file, result.File);
        Assert.AreEqual(rank, result.Rank);
    }
    
    [DataTestMethod]
    [DataRow(-1, 5)]
    [DataRow(8, 7)]
    public void GetSquare_WithInvalidCoordinates_ThrowsException(int file, int rank)
    {
        var board = new Board();
        
        void Act()
        {
            board.GetSquare(file, rank);
        }

        Assert.ThrowsException<OutOfBoardException>((Action) Act);
    }
    
    
    
    
}
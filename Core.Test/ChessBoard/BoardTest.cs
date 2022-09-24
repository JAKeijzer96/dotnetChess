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
    public void GetSquare_WithValidFileAndRank_ReturnsSquare()
    {
        var board = new Board();

        var result = board.GetSquare(1, 7);

        Assert.AreEqual(1, result.File);
        Assert.AreEqual(7, result.Rank);
    }

    [DataTestMethod]
    [DataRow(-1, 5)]
    [DataRow(8, 7)]
    public void GetSquare_WithInvalidFile_ThrowsException(int file, int rank)
    {
        var board = new Board();

        void Act()
        {
            board.GetSquare(file, rank);
        }

        var exception = Assert.ThrowsException<OutOfBoardException>((Action) Act);
        Assert.AreEqual($"File {file} is out of board. Must be between 0 and 7", exception.Message);
    }

    [DataTestMethod]
    [DataRow(3, -2)]
    [DataRow(4, 8)]
    public void GetSquare_WithInvalidRank_ThrowsException(int file, int rank)
    {
        var board = new Board();

        void Act()
        {
            board.GetSquare(file, rank);
        }

        var exception = Assert.ThrowsException<OutOfBoardException>((Action) Act);
        Assert.AreEqual($"Rank {rank} is out of board. Must be between 0 and 7", exception.Message);
    }

    [DataTestMethod]
    [DataRow("e4", 4, 3)]
    [DataRow("b7", 1, 6)]
    [DataRow("h8", 7, 7)]
    public void GetSquare_WithValidSquareName_ReturnsThatSquare(string squareName, int file, int rank)
    {
        var board = new Board();

        var square = board.GetSquare(squareName);

        Assert.AreEqual(file, square.File);
        Assert.AreEqual(rank, square.Rank);
    }

    [TestMethod]
    public void GetSquare_Null_ThrowsArgumentNullException()
    {
        var board = new Board();

        void Act()
        {
            board.GetSquare(null!);
        }

        var exception = Assert.ThrowsException<ArgumentNullException>((Action) Act);
        Assert.AreEqual("Value cannot be null. (Parameter 'squareName')", exception.Message);
    }
    
    [TestMethod]
    public void GetSquare_StringWithLength3_ThrowsArgumentException()
    {
        var board = new Board();

        void Act()
        {
            board.GetSquare("e44");
        }

        var exception = Assert.ThrowsException<ArgumentException>((Action) Act);
        Assert.AreEqual("Invalid square: e44", exception.Message);
    }

    [TestMethod]
    public void Board_WithStartingPosition_PutsWhiteKingOnE1()
    {
        var board = new Board();
        var expectedPiece = new King(Color.White);

        var actualPiece = board.GetSquare("e1").Piece;

        Assert.IsNotNull(actualPiece);
        Assert.AreEqual(expectedPiece.Color, actualPiece.Color);
        Assert.AreEqual(expectedPiece.Name, actualPiece.Name);
    }

    [TestMethod]
    public void Board_WithStartingPosition_PutsBlackQueenOnD8()
    {
        var board = new Board();
        var expectedPiece = new Queen(Color.Black);

        var actualPiece = board.GetSquare("d8").Piece;

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
}
using System;
using Core.ChessBoard;
using Core.Exceptions;
using Core.Pieces;
using Core.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.ChessBoard;

[TestClass]
public class SquareTests
{

    [DataTestMethod]
    [DataRow(0, 0)]
    [DataRow(7, 7)]
    [DataRow(1, 4)]
    [DataRow(6, 3)]
    public void Constructor_WithValidInput_CreatesSquare(int file, int rank)
    {
        var square = new Square(file, rank);

        Assert.AreEqual(file, square.File);
        Assert.AreEqual(rank, square.Rank);
    }
    
    [DataTestMethod]
    [DataRow(8, 0)]
    [DataRow(-1, 5)]
    [DataRow('e', 4)]
    public void Constructor_WithInvalidFile_ThrowsException(int file, int rank)
    {
        void Act()
        {
            var square = new Square(file, rank);
        }
        
        var exception = Assert.ThrowsException<OutOfBoardException>((Action)Act);
        Assert.AreEqual($"File {file} is out of board. Must be between 0 and 7", exception.Message);
    }
    
    [DataTestMethod]
    [DataRow(0, 8)]
    [DataRow(7, -1)]
    [DataRow(2, '4')]
    public void Constructor_WithInvalidRank_ThrowsException(int file, int rank)
    {
        void Act()
        {
            var square = new Square(file, rank);
        }
        
        var exception = Assert.ThrowsException<OutOfBoardException>((Action)Act);
        Assert.AreEqual($"Rank {rank} is out of board. Must be between 0 and 7", exception.Message);
    }
    
    [TestMethod]
    public void IsOccupied_OfEmptySquare_IsFalse()
    {
        var square = new Square(0, 0);

        Assert.IsFalse(square.IsOccupied());
    }
    
    [TestMethod]
    public void IsOccupied_OfOccupiedSquare_IsTrue()
    {
        var square = new Square(0, 0, new Bishop(Color.Black));

        Assert.IsTrue(square.IsOccupied());
    }
}
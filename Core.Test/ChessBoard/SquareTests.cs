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
    [DataRow(0, 8)]
    [DataRow(-1, 0)]
    [DataRow('e', 4)]
    public void Constructor_WithInvalidInput_ThrowsException(int file, int rank)
    {
        Assert.ThrowsException<OutOfBoardException>(() => new Square(file, rank));
    }
    
    [TestMethod]
    public void IsOccupied_WithEmptySquare_ReturnsFalse()
    {
        var square = new Square(0, 0);

        Assert.IsFalse(square.IsOccupied());
    }
    
    [TestMethod]
    public void IsOccupied_WithOccupiedSquare_ReturnsTrue()
    {
        var square = new Square(0, 0, new Bishop(Color.Black));

        Assert.IsTrue(square.IsOccupied());
    }
}
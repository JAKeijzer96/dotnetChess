using Core.ChessBoard;
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
        var square = new Square((File) file, (Rank) rank);

        Assert.AreEqual(file, square.File);
        Assert.AreEqual(rank, square.Rank);
    }

    [TestMethod]
    public void IsOccupied_OfEmptySquare_IsFalse()
    {
        var square = new Square(File.A, Rank.First);

        Assert.IsFalse(square.IsOccupied());
    }

    [TestMethod]
    public void IsOccupied_OfSquareWithPiece_IsTrue()
    {
        var square = new Square(File.G, Rank.Seventh, new Bishop(Color.Black));

        Assert.IsTrue(square.IsOccupied());
    }

    [DataTestMethod]
    [DataRow(0, 0, "a1")]
    [DataRow(3, 7, "d8")]
    public void ToString_ReturnsExpectedValue(int file, int rank, string expected)
    {
        var square = new Square((File) file, (Rank) rank);
        
        Assert.AreEqual(expected, square.ToString());
    }

    [TestMethod]
    public void GetHashCode_OfEqualEmptySquares_AreEqual()
    {
        var hashcode1 = new Square(File.F, Rank.Seventh).GetHashCode();
        var hashcode2 = new Square(File.F, Rank.Seventh).GetHashCode();

        Assert.AreEqual(hashcode1, hashcode2);
    }

    [TestMethod]
    public void GetHashCode_OfEqualSquaresWithEqualPiece_AreEqual()
    {
        var hashcode1 = new Square(File.C, Rank.Second, new Pawn(Color.White)).GetHashCode();
        var hashcode2 = new Square(File.C, Rank.Second, new Pawn(Color.White)).GetHashCode();

        Assert.AreEqual(hashcode1, hashcode2);
    }

    [TestMethod]
    public void GetHashCode_OfEqualSquaresWithUnequalPieces_AreNotEqual()
    {
        var hashcode1 = new Square(File.D, Rank.Eighth, new Rook(Color.Black)).GetHashCode();
        var hashcode2 = new Square(File.D, Rank.Eighth, new Rook(Color.White)).GetHashCode();

        Assert.AreNotEqual(hashcode1, hashcode2);
    }

    [TestMethod]
    public void GetHashCode_OfUnequalSquaresWithEqualPieces_AreNotEqual()
    {
        var hashcode1 = new Square(File.D, Rank.Third, new Knight(Color.Black)).GetHashCode();
        var hashcode2 = new Square(File.H, Rank.Fourth, new Knight(Color.Black)).GetHashCode();

        Assert.AreNotEqual(hashcode1, hashcode2);
    }

    [TestMethod]
    public void EqualsOperator_ForEqualSquaresWithEqualPieces_ReturnsTrue()
    {
        var square1 = new Square(File.C, Rank.Sixth, new Queen(Color.Black));
        var square2 = new Square(File.C, Rank.Sixth, new Queen(Color.Black));

        var result = square1 == square2;

        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void EqualsOperator_ForEqualEmptySquares_ReturnsTrue()
    {
        var square1 = new Square(File.E, Rank.Fifth);
        var square2 = new Square(File.E, Rank.Fifth);

        var result = square1 == square2;

        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void EqualsOperator_ForEqualSquaresWithUnequalPieces_ReturnsFalse()
    {
        var square1 = new Square(File.E, Rank.First, new King(Color.White));
        var square2 = new Square(File.E, Rank.First, new Rook(Color.White));

        var result = square1 == square2;

        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void EqualsOperator_ForNullSquares_ReturnsTrue()
    {
        Square? square1 = null;
        Square? square2 = null;

        var result = square1 == square2;

        Assert.AreEqual(true, result);
    }
}
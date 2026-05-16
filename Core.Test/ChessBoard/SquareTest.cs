using System.Threading.Tasks;
using Core.ChessBoard;
using Core.Pieces;
using Core.Shared;

namespace Core.Test.ChessBoard;

public class SquareTests
{
    [Test]
    [Arguments(0, 0)]
    [Arguments(7, 7)]
    [Arguments(1, 4)]
    [Arguments(6, 3)]
    public async Task Constructor_WithValidInput_CreatesSquare(int file, int rank)
    {
        var square = new Square((File) file, (Rank) rank);

        await Assert.That(square.File).IsEqualTo((File)file);
        await Assert.That(square.Rank).IsEqualTo((Rank)rank);
    }

    [Test]
    public async Task IsOccupied_OfEmptySquare_IsFalse()
    {
        var square = new Square(File.A, Rank.First);

        await Assert.That(square.IsOccupied()).IsFalse();
    }

    [Test]
    public async Task IsOccupied_OfSquareWithPiece_IsTrue()
    {
        var square = new Square(File.G, Rank.Seventh, new Bishop(Color.Black));

        await Assert.That(square.IsOccupied()).IsTrue();
    }

    [Test]
    [Arguments(0, 0, "a1")]
    [Arguments(3, 7, "d8")]
    public async Task ToString_ReturnsExpectedValue(int file, int rank, string expected)
    {
        var square = new Square((File) file, (Rank) rank);

        await Assert.That(square.ToString()).IsEqualTo(expected);
    }

    [Test]
    public async Task GetHashCode_OfEqualEmptySquares_AreEqual()
    {
        var hashcode1 = new Square(File.F, Rank.Seventh).GetHashCode();
        var hashcode2 = new Square(File.F, Rank.Seventh).GetHashCode();

        await Assert.That(hashcode1).IsEqualTo(hashcode2);
    }

    [Test]
    public async Task GetHashCode_OfEqualSquaresWithEqualPiece_AreEqual()
    {
        var hashcode1 = new Square(File.C, Rank.Second, new Pawn(Color.White)).GetHashCode();
        var hashcode2 = new Square(File.C, Rank.Second, new Pawn(Color.White)).GetHashCode();

        await Assert.That(hashcode1).IsEqualTo(hashcode2);
    }

    [Test]
    public async Task GetHashCode_OfEqualSquaresWithUnequalPieces_AreNotEqual()
    {
        var hashcode1 = new Square(File.D, Rank.Eighth, new Rook(Color.Black)).GetHashCode();
        var hashcode2 = new Square(File.D, Rank.Eighth, new Rook(Color.White)).GetHashCode();

        await Assert.That(hashcode1).IsNotEqualTo(hashcode2);
    }

    [Test]
    public async Task GetHashCode_OfUnequalSquaresWithEqualPieces_AreNotEqual()
    {
        var hashcode1 = new Square(File.D, Rank.Third, new Knight(Color.Black)).GetHashCode();
        var hashcode2 = new Square(File.H, Rank.Fourth, new Knight(Color.Black)).GetHashCode();

        await Assert.That(hashcode1).IsNotEqualTo(hashcode2);
    }

    [Test]
    public async Task EqualsOperator_ForEqualSquaresWithEqualPieces_ReturnsTrue()
    {
        var square1 = new Square(File.C, Rank.Sixth, new Queen(Color.Black));
        var square2 = new Square(File.C, Rank.Sixth, new Queen(Color.Black));

        var result = square1 == square2;

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task EqualsOperator_ForEqualEmptySquares_ReturnsTrue()
    {
        var square1 = new Square(File.E, Rank.Fifth);
        var square2 = new Square(File.E, Rank.Fifth);

        var result = square1 == square2;

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task EqualsOperator_ForEqualSquaresWithUnequalPieces_ReturnsFalse()
    {
        var square1 = new Square(File.E, Rank.First, new King(Color.White));
        var square2 = new Square(File.E, Rank.First, new Rook(Color.White));

        var result = square1 == square2;

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task EqualsOperator_ForNullSquares_ReturnsTrue()
    {
        Square? square1 = null;
        Square? square2 = null;

        var result = square1 == square2;

        await Assert.That(result).IsTrue();
    }
}
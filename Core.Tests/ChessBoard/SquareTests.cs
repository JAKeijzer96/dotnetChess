using Core.ChessBoard;
using Core.Exceptions;
using Core.Pieces;
using Core.Shared;
using Xunit;

namespace Core.Tests.ChessBoard;

public class SquareTests
{

    [Theory]
    [InlineData(0, 0)]
    [InlineData(7, 7)]
    [InlineData(1, 4)]
    [InlineData(6, 3)]
    public void Constructor_WithValidInput_CreatesSquare(int file, int rank)
    {
        var square = new Square(file, rank);

        Assert.Equal(file, square.File);
        Assert.Equal(rank, square.Rank);
    }
    
    [Theory]
    [InlineData(8, 0)]
    [InlineData(0, 8)]
    [InlineData(-1, 0)]
    [InlineData('e', 4)]
    public void Constructor_WithInvalidInput_ThrowsException(int file, int rank)
    {
        Assert.Throws<OutOfBoardException>(() => new Square(file, rank));
    }
    
    [Fact]
    public void IsOccupied_WithEmptySquare_ReturnsFalse()
    {
        var square = new Square(0, 0);

        Assert.False(square.IsOccupied());
    }
    
    [Fact]
    public void IsOccupied_WithOccupiedSquare_ReturnsTrue()
    {
        var square = new Square(0, 0, new Bishop(Color.Black));

        Assert.True(square.IsOccupied());
    }
}
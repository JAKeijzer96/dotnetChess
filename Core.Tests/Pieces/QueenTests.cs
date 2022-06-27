using Core.Pieces;
using Core.Shared;
using Xunit;

namespace Core.Tests.Pieces;

public class QueenTests
{
    [Theory]
    [InlineData(Color.White, 'Q')]
    [InlineData(Color.Black, 'q')]
    public void Constructor_SetsNameBasedOnColor(Color color, char expected)
    {
        var queen = new Queen(color);
        
        var actual = queen.Name;
        
        Assert.Equal(expected, actual);
    }
}
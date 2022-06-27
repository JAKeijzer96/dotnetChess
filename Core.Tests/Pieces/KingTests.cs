using Core.Pieces;
using Core.Shared;
using Xunit;

namespace Core.Tests.Pieces;

public class KingTests
{
    [Theory]
    [InlineData(Color.White, 'K')]
    [InlineData(Color.Black, 'k')]
    public void Constructor_SetsNameBasedOnColor(Color color, char expected)
    {
        var king = new King(color);
        
        var actual = king.Name;
        
        Assert.Equal(expected, actual);
    }
}
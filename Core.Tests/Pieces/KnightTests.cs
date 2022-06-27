using Core.Pieces;
using Core.Shared;
using Xunit;

namespace Core.Tests.Pieces;

public class KnightTests
{
    [Theory]
    [InlineData(Color.White, 'N')]
    [InlineData(Color.Black, 'n')]
    public void Constructor_SetsNameBasedOnColor(Color color, char expected)
    {
        var knight = new Knight(color);
        
        var actual = knight.Name;
        
        Assert.Equal(expected, actual);
    }
}
using Core.Pieces;
using Core.Shared;
using Xunit;

namespace Core.Tests.Pieces;

public class RookTests
{
    [Theory]
    [InlineData(Color.White, 'R')]
    [InlineData(Color.Black, 'r')]
    public void Constructor_SetsNameBasedOnColor(Color color, char expected)
    {
        var rook = new Rook(color);
        
        var actual = rook.Name;
        
        Assert.Equal(expected, actual);
    }
}
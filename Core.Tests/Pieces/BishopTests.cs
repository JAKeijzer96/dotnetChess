using Core.Pieces;
using Core.Shared;
using Xunit;

namespace Core.Tests.Pieces;

public class BishopTests
{
    [Theory]
    [InlineData(Color.White, 'B')]
    [InlineData(Color.Black, 'b')]
    public void Constructor_SetsNameBasedOnColor(Color color, char expected)
    {
        var bishop = new Bishop(color);
        
        var actual = bishop.Name;
        
        Assert.Equal(expected, actual);
    }
}
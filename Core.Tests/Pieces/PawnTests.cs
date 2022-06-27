using Core.Pieces;
using Core.Shared;
using Xunit;

namespace Core.Tests.Pieces;

public class PawnTests
{
    [Theory]
    [InlineData(Color.White, 'P')]
    [InlineData(Color.Black, 'p')]
    public void Constructor_SetsNameBasedOnColor(Color color, char expected)
    {
        var pawn = new Pawn(color);
        
        var actual = pawn.Name;
        
        Assert.Equal(expected, actual);
    }
}
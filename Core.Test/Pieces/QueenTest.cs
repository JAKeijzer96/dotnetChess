using System.Threading.Tasks;
using Core.Pieces;
using Core.Shared;

namespace Core.Test.Pieces;

public class QueenTest
{
    [Test]
    [Arguments(Color.White, 'Q')]
    [Arguments(Color.Black, 'q')]
    public async Task Queen_WithColor_HasCorrectNameAndColor(Color color, char name)
    {
        var queen = new Queen(color);
        
        await Assert.That(queen.Name).IsEqualTo(name);
        await Assert.That(queen.Color).IsEqualTo(color);
    }
}
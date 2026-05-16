using System.Threading.Tasks;
using Core.Pieces;
using Core.Shared;

namespace Core.Test.Pieces;

public class KingTest
{
    [Test]
    [Arguments(Color.White, 'K')]
    [Arguments(Color.Black, 'k')]
    public async Task King_WithColor_HasCorrectNameAndColor(Color color, char name)
    {
        var king = new King(color);
        
        await Assert.That(king.Name).IsEqualTo(name);
        await Assert.That(king.Color).IsEqualTo(color);
    }
}
using System.Threading.Tasks;
using Core.Pieces;
using Core.Shared;

namespace Core.Test.Pieces;

public class PawnTest
{
    [Test]
    [Arguments(Color.White, 'P')]
    [Arguments(Color.Black, 'p')]
    public async Task Pawn_WithColor_HasCorrectNameAndColor(Color color, char name)
    {
        var pawn = new Pawn(color);
        
        await Assert.That(pawn.Name).IsEqualTo(name);
        await Assert.That(pawn.Color).IsEqualTo(color);
    }
}
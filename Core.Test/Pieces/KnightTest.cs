using System.Threading.Tasks;
using Core.Pieces;
using Core.Shared;

namespace Core.Test.Pieces;

public class KnightTest
{
    [Test]
    [Arguments(Color.White, 'N')]
    [Arguments(Color.Black, 'n')]
    public async Task Knight_WithColor_HasCorrectNameAndColor(Color color, char name)
    {
        var knight = new Knight(color);
        
        await Assert.That(knight.Name).IsEqualTo(name);
        await Assert.That(knight.Color).IsEqualTo(color);
    }
}
using System.Threading.Tasks;
using Core.Pieces;
using Core.Shared;

namespace Core.Test.Pieces;

public class BishopTest
{
    [Test]
    [Arguments(Color.White, 'B')]
    [Arguments(Color.Black, 'b')]
    public async Task Bishop_WithColor_HasCorrectNameAndColor(Color color, char name)
    {
        var bishop = new Bishop(color);
        
        await Assert.That(bishop.Name).IsEqualTo(name);
        await Assert.That(bishop.Color).IsEqualTo(color);
    }
}
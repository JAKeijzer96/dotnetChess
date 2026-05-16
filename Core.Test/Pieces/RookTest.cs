using System.Threading.Tasks;
using Core.Pieces;
using Core.Shared;

namespace Core.Test.Pieces;

public class RookTest
{
    [Test]
    [Arguments(Color.White, 'R')]
    [Arguments(Color.Black, 'r')]
    public async Task Rook_WithColor_HasCorrectNameAndColor(Color color, char name)
    {
        var rook = new Rook(color);
        
        await Assert.That(rook.Name).IsEqualTo(name);
        await Assert.That(rook.Color).IsEqualTo(color);
    }
}

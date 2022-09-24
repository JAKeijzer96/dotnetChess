using Core.Pieces;
using Core.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.Pieces;

[TestClass]
public class BishopTest
{
    [DataTestMethod]
    [DataRow(Color.White, 'B')]
    [DataRow(Color.Black, 'b')]
    public void Bishop_WithColor_HasCorrectNameAndColor(Color color, char name)
    {
        var bishop = new Bishop(color);
        
        Assert.AreEqual(name, bishop.Name);
        Assert.AreEqual(color, bishop.Color);
    }
}
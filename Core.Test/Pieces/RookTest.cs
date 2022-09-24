using Core.Pieces;
using Core.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.Pieces;

[TestClass]
public class RookTest
{
    [DataTestMethod]
    [DataRow(Color.White, 'R')]
    [DataRow(Color.Black, 'r')]
    public void Rook_WithColor_HasCorrectNameAndColor(Color color, char name)
    {
        var rook = new Rook(color);
        
        Assert.AreEqual(name, rook.Name);
        Assert.AreEqual(color, rook.Color);
    }
}
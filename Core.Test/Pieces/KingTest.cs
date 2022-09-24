using Core.Pieces;
using Core.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.Pieces;

[TestClass]
public class KingTest
{
    [DataTestMethod]
    [DataRow(Color.White, 'K')]
    [DataRow(Color.Black, 'k')]
    public void King_WithColor_HasCorrectNameAndColor(Color color, char name)
    {
        var king = new King(color);
        
        Assert.AreEqual(name, king.Name);
        Assert.AreEqual(color, king.Color);
    }
}
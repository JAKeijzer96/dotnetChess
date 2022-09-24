using Core.Pieces;
using Core.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.Pieces;

[TestClass]
public class KnightTest
{
    [DataTestMethod]
    [DataRow(Color.White, 'N')]
    [DataRow(Color.Black, 'n')]
    public void Knight_WithColor_HasCorrectNameAndColor(Color color, char name)
    {
        var knight = new Knight(color);
        
        Assert.AreEqual(name, knight.Name);
        Assert.AreEqual(color, knight.Color);
    }
}
using Core.Pieces;
using Core.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.Pieces;

[TestClass]
public class QueenTest
{
    [DataTestMethod]
    [DataRow(Color.White, 'Q')]
    [DataRow(Color.Black, 'q')]
    public void Queen_WithColor_HasCorrectNameAndColor(Color color, char name)
    {
        var queen = new Queen(color);
        
        Assert.AreEqual(name, queen.Name);
        Assert.AreEqual(color, queen.Color);
    }
}
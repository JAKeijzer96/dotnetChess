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
    public void Queen_WithColor_HasThatNameBasedOnThatColor(Color color, char expected)
    {
        var queen = new Queen(color);
        
        var actual = queen.Name;
        
        Assert.AreEqual(expected, actual);
    }
}
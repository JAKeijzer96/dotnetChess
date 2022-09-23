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
    public void King_WithColor_HasThatNameBasedOnThatColor(Color color, char expected)
    {
        var king = new King(color);
        
        var actual = king.Name;
        
        Assert.AreEqual(expected, actual);
    }
}
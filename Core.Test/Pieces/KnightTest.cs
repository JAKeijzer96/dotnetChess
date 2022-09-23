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
    public void Knight_WithColor_HasThatNameBasedOnThatColor(Color color, char expected)
    {
        var knight = new Knight(color);
        
        var actual = knight.Name;
        
        Assert.AreEqual(expected, actual);
    }
}
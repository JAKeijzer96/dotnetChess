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
    public void Constructor_SetsNameBasedOnColor(Color color, char expected)
    {
        var knight = new Knight(color);
        
        var actual = knight.Name;
        
        Assert.AreEqual(expected, actual);
    }
}
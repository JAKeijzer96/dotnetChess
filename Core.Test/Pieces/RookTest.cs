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
    public void Constructor_SetsNameBasedOnColor(Color color, char expected)
    {
        var rook = new Rook(color);
        
        var actual = rook.Name;
        
        Assert.AreEqual(expected, actual);
    }
}
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
    public void Constructor_SetsNameBasedOnColor(Color color, char expected)
    {
        var bishop = new Bishop(color);
        
        var actual = bishop.Name;
        
        Assert.AreEqual(expected, actual);
    }
}
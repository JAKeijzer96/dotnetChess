using Core.Pieces;
using Core.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.Pieces;

[TestClass]
public class PieceTest
{
    [TestMethod]
    public void ToString_ReturnsNameAsString()
    {
        Piece piece = new King(Color.Black);

        var result = piece.ToString();
        
        Assert.AreEqual("k", result);
    }
}
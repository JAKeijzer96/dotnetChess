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
    
    [TestMethod]
    public void GetHashCode_OfEqualPieces_AreTheSame()
    {
        var hashcode1 = new King(Color.Black).GetHashCode();
        var hashcode2 = new King(Color.Black).GetHashCode();
        
        Assert.AreEqual(hashcode1, hashcode2);
    }
    
    [TestMethod]
    public void GetHashCode_OfUnequalPieces_AreDifferent()
    {
        var hashcode1 = new King(Color.White).GetHashCode();
        var hashcode2 = new King(Color.Black).GetHashCode();
        
        Assert.AreNotEqual(hashcode1, hashcode2);
    }
    
    [TestMethod]
    public void EqualsOperator_ForEqualPieces_ReturnsTrue()
    {
        Piece left = new King(Color.Black);
        Piece right = new King(Color.Black);
        
        var result = left == right;
        
        Assert.AreEqual(true, result);
    }
    
    [TestMethod]
    public void EqualsOperator_ForUnequalPiecesOfSameColor_ReturnsFalse()
    {
        Piece left = new King(Color.Black);
        Piece right = new Bishop(Color.Black);
        
        var result = left == right;
        
        Assert.AreEqual(false, result);
    }
    
    [TestMethod]
    public void EqualsOperator_ForSamePieceTypeOfDifferentColor_ReturnsFalse()
    {
        Piece left = new Rook(Color.White);
        Piece right = new Rook(Color.Black);
        
        var result = left == right;
        
        Assert.AreEqual(false, result);
    }
    
    
}
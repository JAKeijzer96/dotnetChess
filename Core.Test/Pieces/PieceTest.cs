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
        Piece piece1 = new King(Color.Black);
        Piece piece2 = new King(Color.Black);
        
        var result = piece1 == piece2;
        
        Assert.AreEqual(true, result);
    }
    
    [TestMethod]
    public void EqualsOperator_ForDifferentPieceTypesOfSameColor_ReturnsFalse()
    {
        Piece piece1 = new King(Color.Black);
        Piece piece2 = new Bishop(Color.Black);
        
        var result = piece1 == piece2;
        
        Assert.AreEqual(false, result);
    }
    
    [TestMethod]
    public void EqualsOperator_ForSamePieceTypeOfDifferentColor_ReturnsFalse()
    {
        Piece piece1 = new Rook(Color.White);
        Piece piece2 = new Rook(Color.Black);
        
        var result = piece1 == piece2;
        
        Assert.AreEqual(false, result);
    }
    
    [TestMethod]
    public void EqualsOperator_ForNullPieces_ReturnsTrue()
    {
        Piece? piece1 = null;
        Piece? piece2 = null;
        
        var result = piece1 == piece2;

        Assert.AreEqual(true, result);
    }
    
}
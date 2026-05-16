using System.Threading.Tasks;
using Core.Pieces;
using Core.Shared;

namespace Core.Test.Pieces;

public class PieceTest
{
    [Test]
    public async Task ToString_ReturnsNameAsString()
    {
        Piece piece = new King(Color.Black);

        var result = piece.ToString();
        
        await Assert.That(result).IsEqualTo("k");
    }
    
    [Test]
    public async Task GetHashCode_OfEqualPieces_AreTheSame()
    {
        var hashcode1 = new King(Color.Black).GetHashCode();
        var hashcode2 = new King(Color.Black).GetHashCode();
        
        await Assert.That(hashcode1).IsEqualTo(hashcode2);
    }
    
    [Test]
    public async Task GetHashCode_OfUnequalPieces_AreDifferent()
    {
        var hashcode1 = new King(Color.White).GetHashCode();
        var hashcode2 = new King(Color.Black).GetHashCode();
        
        await Assert.That(hashcode1).IsNotEqualTo(hashcode2);
    }
    
    [Test]
    public async Task EqualsOperator_ForEqualPieces_ReturnsTrue()
    {
        Piece piece1 = new King(Color.Black);
        Piece piece2 = new King(Color.Black);
        
        var result = piece1 == piece2;
        
        await Assert.That(result).IsTrue();
    }
    
    [Test]
    public async Task EqualsOperator_ForDifferentPieceTypesOfSameColor_ReturnsFalse()
    {
        Piece piece1 = new King(Color.Black);
        Piece piece2 = new Bishop(Color.Black);
        
        var result = piece1 == piece2;
        
        await Assert.That(result).IsFalse();
    }
    
    [Test]
    public async Task EqualsOperator_ForSamePieceTypeOfDifferentColor_ReturnsFalse()
    {
        Piece piece1 = new Rook(Color.White);
        Piece piece2 = new Rook(Color.Black);
        
        var result = piece1 == piece2;
        
        await Assert.That(result).IsFalse();
    }
    
    [Test]
    public async Task EqualsOperator_ForNullPieces_ReturnsTrue()
    {
        Piece? piece1 = null;
        Piece? piece2 = null;
        
        var result = piece1 == piece2;

        await Assert.That(result).IsTrue();
    }
    
}
using Core.Pieces;
using Core.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.Pieces;

[TestClass]
public class PawnTest
{
    [DataTestMethod]
    [DataRow(Color.White, 'P')]
    [DataRow(Color.Black, 'p')]
    public void Pawn_WithColor_HasCorrectNameAndColor(Color color, char name)
    {
        var pawn = new Pawn(color);
        
        Assert.AreEqual(name, pawn.Name);
        Assert.AreEqual(color, pawn.Color);
    }
}
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
    public void Pawn_WithColor_HasThatNameBasedOnThatColor(Color color, char expected)
    {
        var pawn = new Pawn(color);
        
        var actual = pawn.Name;
        
        Assert.AreEqual(expected, actual);
    }
}
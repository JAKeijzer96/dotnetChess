using System;
using Core.Pieces;
using Core.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.Pieces;

[TestClass]
public class PieceFactoryTest
{
    [DataTestMethod]
    [DataRow('K', typeof(King), Color.White)]
    [DataRow('Q', typeof(Queen), Color.White)]
    [DataRow('R', typeof(Rook), Color.White)]
    [DataRow('B', typeof(Bishop), Color.White)]
    [DataRow('N', typeof(Knight), Color.White)]
    [DataRow('P', typeof(Pawn), Color.White)]
    [DataRow('k', typeof(King), Color.Black)]
    [DataRow('q', typeof(Queen), Color.Black)]
    [DataRow('r', typeof(Rook), Color.Black)]
    [DataRow('b', typeof(Bishop), Color.Black)]
    [DataRow('n', typeof(Knight), Color.Black)]
    [DataRow('p', typeof(Pawn), Color.Black)]
    public void CreatePiece_CreatesPieceWithCorrectTypeAndColor(char pieceChar, Type expectedType, Color expectedColor)
    {
        var result = PieceFactory.CreatePiece(pieceChar);
        
        Assert.IsInstanceOfType(result, expectedType);
        Assert.AreEqual(expectedColor, result.Color);
    }
    
    [TestMethod]
    public void CreatePiece_WithIncorrectChar_ThrowsArgumentException()
    {
        void Act()
        {
            PieceFactory.CreatePiece('V');
        }
        
        var exception = Assert.ThrowsException<ArgumentException>((Action)Act);
        Assert.AreEqual("Invalid piece character: 'V'", exception.Message);
    }
}
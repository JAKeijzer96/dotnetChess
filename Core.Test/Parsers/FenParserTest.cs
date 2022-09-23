using System;
using Core.ChessGame;
using Core.Exceptions;
using Core.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.Parsers;

[TestClass]
public class FenParserTest
{
    private const string StartingPositionFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    [TestMethod]
    public void Parse_Null_ThrowsArgumentNullException()
    {
        string? fen = null;
        
        void Act()
        {
            var result = FenParser.Parse(fen);
        }

        Assert.ThrowsException<ArgumentNullException>((Action)Act);
    }
    
    [DataTestMethod]
    [DataRow("rnbqkbnr pppppppp 8 8 8 8 PPPPPPPP RNBQKBNR w KQkq - 0 1")]
    [DataRow("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR")]
    [DataRow("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e0 1")]
    [DataRow("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNRwKQkqc602")]
    public void Parse_FenWithWrongAmountOfSpaces_ThrowsInvalidFenException(string fen)
    {
        void Act()
        {
            var result = FenParser.Parse(fen);
        }

        var exception = Assert.ThrowsException<InvalidFenException>((Action)Act);
        Assert.AreEqual("FEN string must have 6 parts", exception.Message);
    }
    
    
    
    
}
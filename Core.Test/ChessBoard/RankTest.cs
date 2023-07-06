using System;
using Core.ChessBoard;
using Core.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.ChessBoard;

[TestClass]
public class RankTest
{
    [TestMethod]
    public void ParseChar_WithValidChar_ReturnsRank()
    {
        var actual = Rank.ParseChar('2');
        
        Assert.AreEqual(Rank.Second, actual);
    }

    [TestMethod]
    public void ParseChar_WithInvalidChar_ThrowsOutOfBoardException()
    {
        void Act() => Rank.ParseChar('0');

        var exception = Assert.ThrowsException<OutOfBoardException>((Action) Act);
        Assert.AreEqual("Rank -1 is out of board (must be between 0 and 7).", exception.Message);
    }

    [DataTestMethod]
    [DataRow(5)] // Up
    [DataRow(1)] // Down
    public void DistanceTo_OtherRank_ReturnsExpectedDistance(int rankValue)
    {
        var rank = (Rank) rankValue;

        var actual = Rank.Fourth.DistanceTo(rank);
        
        Assert.AreEqual(2, actual);
    }

    [TestMethod]
    public void Increment_WhenRankIsEighth_ThrowsOutOfBoardException()
    {
        void Act()
        {
            #pragma warning disable S1854
            var rank = Rank.Eighth;
            _ = ++rank;
            #pragma warning restore S1854
        }

        var exception = Assert.ThrowsException<OutOfBoardException>((Action) Act);
        Assert.AreEqual("Rank 8 is out of board (must be between 0 and 7).", exception.Message);
    }

    [TestMethod]
    public void Decrement_WhenRankIsFirst_ThrowsOutOfBoardException()
    {
        void Act()
        {
            #pragma warning disable S1854
            var rank = Rank.First;
            _ = --rank;
            #pragma warning restore S1854
        }
        
        var exception = Assert.ThrowsException<OutOfBoardException>((Action) Act);
        Assert.AreEqual("Rank -1 is out of board (must be between 0 and 7).", exception.Message);
    }
}
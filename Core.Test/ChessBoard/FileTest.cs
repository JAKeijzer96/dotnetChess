using System;
using Core.ChessBoard;
using Core.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.ChessBoard;

[TestClass]
public class FileTest
{
    [TestMethod]
    public void ParseChar_WithValidChar_ReturnsFile()
    {
        var actual = File.ParseChar('c');
        
        Assert.AreEqual(File.C, actual);
    }

    [TestMethod]
    public void ParseChar_WithInvalidChar_ThrowsOutOfBoardException()
    {
        void Act() => File.ParseChar('i');

        var exception = Assert.ThrowsException<OutOfBoardException>((Action) Act);
        Assert.AreEqual("File 8 is out of board (must be between 0 and 7).", exception.Message);
    }

    [DataTestMethod]
    [DataRow(5)] // To the right
    [DataRow(1)] // To the left
    public void DistanceTo_OtherFile_ReturnsExpectedDistance(int fileValue)
    {
        var file = (File) fileValue;

        var actual = File.D.DistanceTo(file);
        
        Assert.AreEqual(2, actual);
    }

    [TestMethod]
    public void Increment_WhenFileIsH_ThrowsOutOfBoardException()
    {
        void Act()
        {
            #pragma warning disable S1854
            var file = File.H;
            _ = ++file;
            #pragma warning restore S1854
        }

        var exception = Assert.ThrowsException<OutOfBoardException>((Action) Act);
        Assert.AreEqual("File 8 is out of board (must be between 0 and 7).", exception.Message);
    }

    [TestMethod]
    public void Decrement_WhenFileIsA_ThrowsOutOfBoardException()
    {
        void Act()
        {
            #pragma warning disable S1854
            var file = File.A;
            _ = --file;
            #pragma warning restore S1854
        }
        
        var exception = Assert.ThrowsException<OutOfBoardException>((Action) Act);
        Assert.AreEqual("File -1 is out of board (must be between 0 and 7).", exception.Message);
    }
}
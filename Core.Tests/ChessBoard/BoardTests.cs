using System;
using System.Collections.Generic;
using Core.ChessBoard;
using Core.Exceptions;
using Core.Pieces;
using Core.Shared;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Core.Tests.ChessBoard;

public class BoardTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Board _board;

    public BoardTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _board = new Board();
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 7)]
    public void GetSquare_WithValidCoordinates_ReturnsSquare(int file, int rank)
    {
        var square = _board.GetSquare(file, rank);
        Assert.Equal(file, square.File);
        Assert.Equal(rank, square.Rank);
    }
    
    [Theory]
    [InlineData(-1, 5)]
    [InlineData(7, 8)]
    public void GetSquare_WithInvalidCoordinates_ThrowsException(int file, int rank)
    {
        Assert.Throws<OutOfBoardException>(() => _board.GetSquare(file, rank));
    }
    
    [Fact]
    public void Constructor_PutsWhiteKingOnE1()
    {
        var expectedPiece = new King(Color.White);
        var square = _board.GetSquare(4, 0);
        var actualPiece = square.Piece;
        Assert.NotNull(actualPiece);
        Assert.Equal(expectedPiece.Name, actualPiece!.Name);
    }
    
    [Fact]
    public void Constructor_PutsBlackQueenOnD8()
    {
        var expectedPiece = new Queen(Color.Black);
        var square = _board.GetSquare(3, 7);
        var actualPiece = square.Piece;
        Assert.NotNull(actualPiece);
        Assert.Equal(expectedPiece.Name, actualPiece!.Name);
    }
    
    
}
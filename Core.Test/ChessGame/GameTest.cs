using System;
using Core.ChessBoard;
using Core.ChessGame;
using Core.Exceptions;
using Core.Parsers;
using Core.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.ChessGame;

[TestClass]
public class GameTest
{
    [DataTestMethod]
    [DataRow(Color.White, "e7", "e5")]
    [DataRow(Color.Black, "e2", "e4")]
    public void MakeMove_MovingOpponentsPiece_ReturnsFalse(Color turn, string from, string to)
    {
        // Arrange
        var sut = new Game(new Board(), turn, "KQkq", null, 0, 1);

        // Act
        var result = sut.MakeMove(from, to);

        // Assert
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void MakeMove_WhenPieceIsNull_ReturnsFalse()
    {
        // Arrange
        var sut = new Game(new Board(), Color.White, "KQkq", null, 0, 1);

        // Act
        var result = sut.MakeMove("a4", "a5");

        // Assert
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void MakeMove_WithInvalidMove_ReturnsFalse()
    {
        // Arrange
        var sut = new Game(new Board(), Color.White, "KQkq", null, 0, 1);

        // Act
        var result = sut.MakeMove("e2", "b4");

        // Assert
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void MakeMove_WithValidMove_ReturnsTrue()
    {
        // Arrange
        var sut = new Game(new Board(), Color.White, "KQkq", null, 0, 1);

        // Act
        var result = sut.MakeMove("e2", "e4");

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void EndTurn_AfterWhiteMoves_TurnIsBlack()
    {
        // Arrange
        var sut = new Game();

        // Act
        sut.MakeMove("e2", "e4");

        // Assert
        Assert.AreEqual(Color.Black, sut.Turn);
    }
    
    [TestMethod]
    public void EndTurn_AfterBlackMoves_TurnIsWhite()
    {
        // Arrange
        var sut = new Game(new Board("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR"),
            Color.Black, "KQkq", null, 0, 1);

        // Act
        sut.MakeMove("e7", "e5");

        // Assert
        Assert.AreEqual(Color.White, sut.Turn);
    }
    
    [TestMethod]
    public void EndTurn_AfterBlackMoves_FullMoveCountIncreases()
    {
        // Arrange
        var sut = new Game(new Board("r1bqkb1r/pppp1ppp/2n2n2/4p3/2B1P3/2N2N2/PPPP1PPP/R1BQK2R"),
            Color.Black, "KQkq", null, 5, 4);

        // Act
        sut.MakeMove("f6", "e4");

        // Assert
        Assert.AreEqual(5, sut.FullMoveCount);
    }

    [TestMethod]
    public void UpdateHalfMoveCount_AfterPawnMove_ResetsHalfMoveCount()
    {
        // Arrange
        var sut = new Game(new Board("rnbqkbnr/ppp1pppp/8/8/8/2N2N2/PPPP1PPP/R1BQKB1R"),
            Color.Black, "KQkq", null, 3, 4);

        // Act
        sut.MakeMove("e7", "e6");

        // Assert
        Assert.AreEqual(0, sut.HalfMoveCount);
    }
    
    [TestMethod]
    public void UpdateHalfMoveCount_AfterCapture_ResetsHalfMoveCount()
    {
        // Arrange
        var sut = new Game(new Board("r1bqkbnr/ppp2ppp/2n1p3/1B6/8/2N2N2/PPPP1PPP/R1BQK2R"),
            Color.White, "KQkq", null, 2, 6);

        // Act
        sut.MakeMove("b5", "c6");

        // Assert
        Assert.AreEqual(0, sut.HalfMoveCount);
    }
    
    [TestMethod]
    public void UpdateHalfMoveCount_AfterPieceMoveWithoutCapture_IncrementsHalfMoveCount()
    {
        // Arrange
        var sut = new Game(new Board("r1bqkbnr/ppp2ppp/2n1p3/1B6/8/2N2N2/PPPP1PPP/R1BQ1RK1"),
            Color.Black, "kq", null, 3, 6);

        // Act
        sut.MakeMove("g8", "f6");

        // Assert
        Assert.AreEqual(4, sut.HalfMoveCount);
    }

    #region EnPassant

    [TestMethod]
    public void MakeMove_WhenMoveIsEnPassant_ReturnsTrue()
    {
        // Arrange
        var board = new Board("rnbqkb1r/ppp1pppp/8/8/3pPn2/8/PPPP1PPP/RNBQKBNR");
        var enPassantSquare = board["e3"];
        var sut = new Game(board, Color.Black, "KQkq", enPassantSquare, 0, 5);
        
        // Act
        var result = sut.MakeMove("d4", "e3");
        
        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void MakeMove_WhenMoveIsEnPassant_ResetsEnPassantSquare()
    {
        // Arrange
        var board = new Board("rnbqkb1r/ppp1pppp/8/8/3pPn2/8/PPPP1PPP/RNBQKBNR");
        var enPassantSquare = board["e3"];
        var sut = new Game(board, Color.Black, "KQkq", enPassantSquare, 0, 5);
        
        // Act
        sut.MakeMove("d4", "e3");
        
        // Assert
        Assert.IsNull(sut.EnPassant);
    }
    
    [TestMethod]
    public void MakeMove_WhenMoveIsEnPassant_RemovesCapturedPieceFromBoard()
    {
        // Arrange
        var board = new Board("rnbqkb1r/ppp1pppp/8/8/3pPn2/8/PPPP1PPP/RNBQKBNR");
        var enPassantSquare = board["e3"];
        var sut = new Game(board, Color.Black, "KQkq", enPassantSquare, 0, 5);
        
        // Act
        sut.MakeMove("d4", "e3");
        
        // Assert
        Assert.IsNull(sut.Board["e4"].Piece);
    }
    
    [TestMethod]
    public void MakeMove_WhenMoveIsEnPassantWithNonPawn_ReturnsFalse()
    {
        // Arrange
        var board = new Board("rnbqkb1r/ppp1pppp/8/8/3pPn2/8/PPPP1PPP/RNBQKBNR");
        var enPassantSquare = board["e3"];
        var sut = new Game(board, Color.Black, "KQkq", enPassantSquare, 0, 5);
        
        // Act
        var result = sut.MakeMove("f4", "e3");
        
        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region PawnPromotion

    [TestMethod]
    public void MakeMove_WhenWhitePawnMovesToEighthRank_IsPromotedToGivenPiece()
    {
        // Arrange
        var sut = new Game(new Board("8/P7/8/8/8/8/K1k3p1/8"), Color.White, "-", null, 0, 1);

        // Act
        sut.MakeMove("a7", "a8", 'N');

        // Assert
        var actual = sut.Board["a8"].Piece!.Name;
        Assert.AreEqual('N', actual);
    }
    
    [TestMethod]
    public void MakeMove_WhenBlackPawnMovesToFirstRank_IsPromotedToGivenPiece()
    {
        // Arrange
        var sut = new Game(new Board("8/P7/8/8/8/8/K1k3p1/8"), Color.Black, "-", null, 0, 1);

        // Act
        sut.MakeMove("g2", "g1", 'b');

        // Assert
        var actual = sut.Board["g1"].Piece!.Name;
        Assert.AreEqual('b', actual);
    }
    
    [DataTestMethod]
    [DataRow('\0')] // Default char value
    [DataRow('B')] // Opposite color
    public void MakeMove_WhenPawnMovesToLastRankWithInvalidPromotionPieceChar_ThrowsInvalidPromotionException(char promotionPieceChar)
    {
        // Arrange
        var sut = new Game(new Board("8/P7/8/8/8/8/K1k3p1/8"), Color.Black, "-", null, 0, 1);

        // Act
        void Act() => sut.MakeMove("g2", "g1", promotionPieceChar);

        // Assert
        var exception = Assert.ThrowsException<InvalidPromotionException>((Action) Act);
        Assert.AreEqual($"Invalid promotion character {promotionPieceChar} for color Black", exception.Message);
    }
    #endregion
}
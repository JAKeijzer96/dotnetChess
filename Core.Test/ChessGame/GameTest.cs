using System;
using Core.ChessBoard;
using Core.ChessGame;
using Core.Exceptions;
using Core.Parsers;
using Core.Pieces;
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

    #region Castling
    
    [DataTestMethod]
    [DataRow(0, "e1", "g1", "f1", "g1", "h1")]
    [DataRow(0, "e1", "b1", "d1", "c1", "a1")]
    [DataRow(1, "e8", "h8", "f8", "g8", "h8")]
    [DataRow(1, "e8", "c8", "d8", "c8", "a8")]
    public void MakeMove_CastlingWhenValid_MovesKingAndRook(int turn, string kingSquare, string destinationSquare,
                                            string rookEndSquare, string kingEndSquare, string rookStartSquare)
    {
        // Arrange
        var board = new Board("r3k2r/8/8/8/8/8/8/R3K2R");
        var sut = new Game(board, (Color)turn, "KQkq", null, 0, 1);

        // Act
        sut.MakeMove(kingSquare, destinationSquare);

        // Assert
        Assert.IsTrue(board[kingSquare].Piece is null);
        Assert.IsTrue(board[rookEndSquare].Piece is Rook);
        Assert.IsTrue(board[kingEndSquare].Piece is King);
        Assert.IsTrue(board[rookStartSquare].Piece is null);
    }
    
    [DataTestMethod]
    [DataRow(0, "KQkq", "e1", "g1", "kq")]
    [DataRow(1, "Kkq", "e8", "a8", "K")]
    [DataRow(0, "KQ", "e1", "b1", "-")]
    public void MakeMove_Castling_UpdatesCastlingProperty(int turn, string castling, string from, string to, string expectedCastlingValue)
    {
        // Arrange
        var board = new Board("r3k2r/8/8/8/8/8/8/R3K2R");
        var sut = new Game(board, (Color)turn, castling, null, 0, 1);

        // Act
        sut.MakeMove(from, to);

        // Assert
        Assert.AreEqual(expectedCastlingValue, sut.Castling);
    }

    [DataTestMethod]
    [DataRow(0, "kq", "e1", "g1")]
    [DataRow(1, "k", "e8", "a8")]
    [DataRow(0, "-", "e1", "b1")]
    public void MakeMove_CastlingAfterKingOrRookHasMoved_ThrowsInvalidCastlingException
        (int turn, string castling, string from, string to)
    {
        // Arrange
        var board = new Board("r3k2r/8/8/8/8/8/8/R3K2R");
        var sut = new Game(board, (Color)turn, castling, null, 0, 1);

        // Act
        void Act() {
            sut.MakeMove(from, to);
        }

        // Assert
        var exception = Assert.ThrowsException<InvalidCastlingException>((Action) Act);
        Assert.AreEqual($"Cannot castle from {from} to {to} because the king and/or rook have moved (Castling string: {castling}).", exception.Message);
    }

    [DataTestMethod]
    [DataRow(0, "e1", "h1", "f")]
    [DataRow(0, "e1", "c1", "c")]
    [DataRow(1, "e8", "g8", "g")]
    [DataRow(1, "e8", "a8", "b")]
    public void MakeMove_CastlingWhenBlocked_ThrowsInvalidCastlingException(int turn, string from, string to, string blockedFile)
    {
        // Arrange
        var board = new Board("rB2k1nr/8/8/8/8/8/8/R1N1KB1R");
        var sut = new Game(board, (Color)turn, "KQkq", null, 0, 1);
        
        // Act
        void Act()
        {
            sut.MakeMove(from, to);
        }

        // Assert
        var exception = Assert.ThrowsException<InvalidCastlingException>((Action) Act);
        Assert.AreEqual($"Cannot castle from {from} to {to} because there is a piece blocking on file {blockedFile}", exception.Message);
    }
    
    [DataTestMethod]
    [DataRow(0, "KQ", "e1", "e2", "-")] // Move white king
    [DataRow(0, "KQq", "a1", "a5", "Kq")] // Move white a-file rook
    [DataRow(0, "Kq", "h1", "g1", "q")] // Move white h-file rook
    [DataRow(1, "Qkq", "e8", "d8", "Q")] // Move black king
    [DataRow(1, "q", "a8", "d8", "-")] // Move black a-file rook
    [DataRow(1, "Kk", "h8", "h2", "K")] // Move black h-file rook
    public void TestMethod(int turn, string castling, string from, string to, string expectedCastlingValue)
    {
        // Arrange
        var board = new Board("r3kbnr/ppppppp1/8/8/8/8/1PPP1PPP/RNBQKB1R");
        var sut = new Game(board, (Color) turn, castling, null, 0, 1);

        // Act
        sut.MakeMove(from, to);

        // Assert
        Assert.AreEqual(expectedCastlingValue, sut.Castling);
    }

    #endregion
}
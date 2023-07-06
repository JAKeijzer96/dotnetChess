using System;
using Core.ChessBoard;
using Core.Pieces;
using Core.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.ChessBoard;

[TestClass]
public class BoardTest
{
    [TestMethod]
    public void Board_WithFullFen_ThrowsArgumentException()
    {
        var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        void Act() => _ = new Board(fen);

        var exception = Assert.ThrowsException<ArgumentException>((Action) Act);
        Assert.AreEqual("Constructor only accepts board part of FEN string", exception.Message);
    }
    
    [TestMethod]
    public void Indexer_WithValidFileAndRank_ReturnsSquare()
    {
        var board = new Board();

        var result = board[File.B, Rank.Eighth];

        Assert.AreEqual(1, result.File);
        Assert.AreEqual(7, result.Rank);
    }

    [DataTestMethod]
    [DataRow("e4", 4, 3)]
    [DataRow("b7", 1, 6)]
    [DataRow("h8", 7, 7)]
    public void Indexer_WithValidSquareName_ReturnsThatSquare(string squareName, int file, int rank)
    {
        var board = new Board();

        var square = board[squareName];

        Assert.AreEqual(file, square.File);
        Assert.AreEqual(rank, square.Rank);
    }

    [TestMethod]
    public void Indexer_Null_ThrowsArgumentNullException()
    {
        var board = new Board();

        void Act() => _ = board[null!];

        var exception = Assert.ThrowsException<ArgumentNullException>((Action) Act);
        Assert.AreEqual("Value cannot be null. (Parameter 'squareName')", exception.Message);
    }
    
    [TestMethod]
    public void Indexer_StringWithLength3_ThrowsArgumentException()
    {
        var board = new Board();

        void Act() => _ = board["e44"];

        var exception = Assert.ThrowsException<ArgumentException>((Action) Act);
        Assert.AreEqual("Invalid square: e44", exception.Message);
    }

    [TestMethod]
    public void Board_WithStartingPosition_PutsWhiteKingOnE1()
    {
        var board = new Board();
        var expectedPiece = new King(Color.White);

        var actualPiece = board["e1"].Piece;

        Assert.IsNotNull(actualPiece);
        Assert.AreEqual(expectedPiece.Color, actualPiece.Color);
        Assert.AreEqual(expectedPiece.Name, actualPiece.Name);
    }

    [TestMethod]
    public void Board_WithStartingPosition_PutsBlackQueenOnD8()
    {
        var board = new Board();
        var expectedPiece = new Queen(Color.Black);

        var actualPiece = board["d8"].Piece;

        Assert.IsNotNull(actualPiece);
        Assert.AreEqual(expectedPiece.Color, actualPiece.Color);
        Assert.AreEqual(expectedPiece.Name, actualPiece.Name);
    }

    [TestMethod]
    public void MovePiece_MovesPieceToNewSquare()
    {
        var board = new Board();
        
        board.MovePiece(board["g1"], board["f3"]);
        
        Assert.AreEqual('N', board["f3"].Piece!.Name);
    }
    
    [TestMethod]
    public void MovePiece_RemovesPieceFromOldSquare()
    {
        var board = new Board();
        
        board.MovePiece(board["e2"], board["e4"]);
        
        Assert.IsNull(board["e2"].Piece);
    }
    
    [TestMethod]
    public void MovePiece_ToSquareOccupiedByOpponent_CapturesPiece()
    {
        var board = new Board("rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR");
        
        board.MovePiece(board["e4"], board["d5"]);
        
        Assert.AreEqual('P', board["d5"].Piece!.Name);
    }

    [TestMethod]
    public void ToString_OfBoardWithStartingPosition_ReturnsCorrectString()
    {
        var boardFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

        var board = new Board(boardFen);

        Assert.AreEqual(boardFen, board.ToString());
    }

    [TestMethod]
    public void ToString_OfBoardWithRanksWithEmptySquaresAndPieces_CorrectlyParsesFirstNumberOfEmptySquares()
    {
        // Related to bug where first number of empty squares was not parsed correctly
        // new Board("8/2b5/8/2R5/8/8/k1K5/8").ToString() would return "8/0b5/8/0R5/8/8/k0K5/8"
        var boardFen = "8/2b5/8/2R5/8/8/k1K5/8";

        var board = new Board(boardFen);
        
        Assert.AreEqual(boardFen, board.ToString());
    }
}
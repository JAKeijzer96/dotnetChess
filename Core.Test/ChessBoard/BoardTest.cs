using System;
using System.Threading.Tasks;
using Core.ChessBoard;
using Core.Pieces;
using Core.Shared;

namespace Core.Test.ChessBoard;

public class BoardTest
{
    [Test]
    public async Task Board_WithFullFen_ThrowsArgumentException()
    {
        var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        void Act() => _ = new Board(fen);

        var exception = await Assert.That(Act).Throws<ArgumentException>();
        await Assert.That(exception!.Message).IsEqualTo("Constructor only accepts board part of FEN string");
    }
    
    [Test]
    public async Task Indexer_WithValidFileAndRank_ReturnsSquare()
    {
        var board = new Board();

        var result = board[File.B, Rank.Eighth];

        await Assert.That(result.File).IsEqualTo(1);
        await Assert.That(result.Rank).IsEqualTo(7);
    }

    [Test]
    [Arguments("e4", 4, 3)]
    [Arguments("b7", 1, 6)]
    [Arguments("h8", 7, 7)]
    public async Task Indexer_WithValidSquareName_ReturnsThatSquare(string squareName, int file, int rank)
    {
        var board = new Board();

        var square = board[squareName];

        await Assert.That(square.File).IsEqualTo(file);
        await Assert.That(square.Rank).IsEqualTo(rank);
    }

    [Test]
    public async Task Indexer_Null_ThrowsArgumentNullException()
    {
        var board = new Board();

        void Act() => _ = board[null!];

        var exception = await Assert.That(Act).Throws<ArgumentNullException>();
        await Assert.That(exception!.Message).IsEqualTo("Value cannot be null. (Parameter 'squareName')");
    }
    
    [Test]
    public async Task Indexer_StringWithLength3_ThrowsArgumentException()
    {
        var board = new Board();

        void Act() => _ = board["e44"];

        var exception = await Assert.That(Act).Throws<ArgumentException>();
        await Assert.That(exception!.Message).IsEqualTo("Invalid square: e44");
    }

    [Test]
    public async Task Board_WithStartingPosition_PutsWhiteKingOnE1()
    {
        var board = new Board();
        var expectedPiece = new King(Color.White);

        var actualPiece = board["e1"].Piece;

        await Assert.That(actualPiece).IsNotNull();
        await Assert.That(actualPiece!.Color).IsEqualTo(expectedPiece.Color);
        await Assert.That(actualPiece.Name).IsEqualTo(expectedPiece.Name);
    }

    [Test]
    public async Task Board_WithStartingPosition_PutsBlackQueenOnD8()
    {
        var board = new Board();
        var expectedPiece = new Queen(Color.Black);

        var actualPiece = board["d8"].Piece;

        await Assert.That(actualPiece).IsNotNull();
        await Assert.That(actualPiece!.Color).IsEqualTo(expectedPiece.Color);
        await Assert.That(actualPiece.Name).IsEqualTo(expectedPiece.Name);
    }

    [Test]
    public async Task MovePiece_MovesPieceToNewSquare()
    {
        var board = new Board();
        
        board.MovePiece(board["g1"], board["f3"]);
        
        await Assert.That(board["f3"].Piece!.Name).IsEqualTo('N');
    }
    
    [Test]
    public async Task MovePiece_RemovesPieceFromOldSquare()
    {
        var board = new Board();
        
        board.MovePiece(board["e2"], board["e4"]);
        
        await Assert.That(board["e2"].Piece).IsNull();
    }
    
    [Test]
    public async Task MovePiece_ToSquareOccupiedByOpponent_CapturesPiece()
    {
        var board = new Board("rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR");
        
        board.MovePiece(board["e4"], board["d5"]);
        
        await Assert.That(board["d5"].Piece!.Name).IsEqualTo('P');
    }

    [Test]
    public async Task ToString_OfBoardWithStartingPosition_ReturnsCorrectString()
    {
        var boardFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

        var board = new Board(boardFen);

        await Assert.That(board.ToString()).IsEqualTo(boardFen);
    }

    [Test]
    public async Task ToString_OfBoardWithRanksWithEmptySquaresAndPieces_CorrectlyParsesFirstNumberOfEmptySquares()
    {
        // Related to bug where first number of empty squares was not parsed correctly
        // new Board("8/2b5/8/2R5/8/8/k1K5/8").ToString() would return "8/0b5/8/0R5/8/8/k0K5/8"
        var boardFen = "8/2b5/8/2R5/8/8/k1K5/8";

        var board = new Board(boardFen);
        
        await Assert.That(board.ToString()).IsEqualTo(boardFen);
    }
}
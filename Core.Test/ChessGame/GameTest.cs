using System.Threading.Tasks;
using Core.ChessBoard;
using Core.ChessGame;
using Core.Exceptions;
using Core.Pieces;
using Core.Shared;

namespace Core.Test.ChessGame;

public class GameTest
{
    [Test]
    [Arguments(Color.White, "e7", "e5")]
    [Arguments(Color.Black, "e2", "e4")]
    public async Task MakeMove_MovingOpponentsPiece_ReturnsFalse(Color turn, string from, string to)
    {
        var castlingAvailability = new CastlingAvailability("KQkq");
        var sut = new Game(new Board(), turn, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove(from, to);

        await Assert.That(result).IsFalse();
    }
    
    [Test]
    public async Task MakeMove_WhenPieceIsNull_ReturnsFalse()
    {
        var castlingAvailability = new CastlingAvailability("KQkq");
        var sut = new Game(new Board(), Color.White, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove("a4", "a5");

        await Assert.That(result).IsFalse();
    }
    
    [Test]
    public async Task MakeMove_WithInvalidMove_ReturnsFalse()
    {
        var castlingAvailability = new CastlingAvailability("KQkq");
        var sut = new Game(new Board(), Color.White, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove("e2", "b4");

        await Assert.That(result).IsFalse();
    }
    
    [Test]
    public async Task MakeMove_WithValidMove_ReturnsTrue()
    {
        var castlingAvailability = new CastlingAvailability("KQkq");
        var sut = new Game(new Board(), Color.White, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove("e2", "e4");

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task EndTurn_AfterWhiteMoves_TurnIsBlack()
    {
        var sut = new Game();

        sut.MakeMove("e2", "e4");

        await Assert.That(sut.Turn).IsEqualTo(Color.Black);
    }
    
    [Test]
    public async Task EndTurn_AfterBlackMoves_TurnIsWhite()
    {
        var board = new Board("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR");
        var castlingAvailability = new CastlingAvailability("KQkq");
        var sut = new Game(board, Color.Black, castlingAvailability, null, 0, 1);

        sut.MakeMove("e7", "e5");

        await Assert.That(sut.Turn).IsEqualTo(Color.White);
    }
    
    [Test]
    public async Task EndTurn_AfterBlackMoves_FullMoveCountIncreases()
    {
        var board = new Board("r1bqkb1r/pppp1ppp/2n2n2/4p3/2B1P3/2N2N2/PPPP1PPP/R1BQK2R");
        var castlingAvailability = new CastlingAvailability("KQkq");
        var sut = new Game(board, Color.Black, castlingAvailability, null, 5, 4);

        sut.MakeMove("f6", "e4");

        await Assert.That(sut.FullMoveCount).IsEqualTo(5);
    }

    [Test]
    public async Task UpdateHalfMoveCount_AfterPawnMove_ResetsHalfMoveCount()
    {
        var board = new Board("rnbqkbnr/ppp1pppp/8/8/8/2N2N2/PPPP1PPP/R1BQKB1R");
        var castlingAvailability = new CastlingAvailability("KQkq");
        var sut = new Game(board, Color.Black, castlingAvailability, null, 3, 4);

        sut.MakeMove("e7", "e6");

        await Assert.That(sut.HalfMoveCount).IsEqualTo(0);
    }
    
    [Test]
    public async Task UpdateHalfMoveCount_AfterCapture_ResetsHalfMoveCount()
    {
        var board = new Board("r1bqkbnr/ppp2ppp/2n1p3/1B6/8/2N2N2/PPPP1PPP/R1BQK2R");
        var castlingAvailability = new CastlingAvailability("KQkq");
        var sut = new Game(board, Color.White, castlingAvailability, null, 2, 6);

        sut.MakeMove("b5", "c6");

        await Assert.That(sut.HalfMoveCount).IsEqualTo(0);
    }
    
    [Test]
    public async Task UpdateHalfMoveCount_AfterPieceMoveWithoutCapture_IncrementsHalfMoveCount()
    {
        var board = new Board("r1bqkbnr/ppp2ppp/2n1p3/1B6/8/2N2N2/PPPP1PPP/R1BQ1RK1");
        var castlingAvailability = new CastlingAvailability("kq");
        var sut = new Game(board, Color.Black, castlingAvailability, null, 3, 6);

        sut.MakeMove("g8", "f6");

        await Assert.That(sut.HalfMoveCount).IsEqualTo(4);
    }

    #region EnPassant

    [Test]
    public async Task MakeMove_WhenMoveIsEnPassant_ReturnsTrue()
    {
        var board = new Board("rnbqkb1r/ppp1pppp/8/8/3pPn2/8/PPPP1PPP/RNBQKBNR");
        var castlingAvailability = new CastlingAvailability("KQkq");
        var enPassantSquare = board["e3"];
        var sut = new Game(board, Color.Black, castlingAvailability, enPassantSquare, 0, 5);
        
        var result = sut.MakeMove("d4", "e3");

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task MakeMove_WhenMoveIsEnPassant_ResetsEnPassantSquare()
    {
        var board = new Board("rnbqkb1r/ppp1pppp/8/8/3pPn2/8/PPPP1PPP/RNBQKBNR");
        var castlingAvailability = new CastlingAvailability("KQkq");
        var enPassantSquare = board["e3"];
        var sut = new Game(board, Color.Black, castlingAvailability, enPassantSquare, 0, 5);
        
        sut.MakeMove("d4", "e3");

        await Assert.That(sut.EnPassant).IsNull();
    }

    [Test]
    public async Task MakeMove_WhenMoveIsEnPassant_RemovesCapturedPieceFromBoard()
    {
        var board = new Board("rnbqkb1r/ppp1pppp/8/8/3pPn2/8/PPPP1PPP/RNBQKBNR");
        var castlingAvailability = new CastlingAvailability("KQkq");
        var enPassantSquare = board["e3"];
        var sut = new Game(board, Color.Black, castlingAvailability, enPassantSquare, 0, 5);
        
        sut.MakeMove("d4", "e3");

        await Assert.That(sut.Board["e4"].Piece).IsNull();
    }
    
    [Test]
    public async Task MakeMove_WhenMoveIsEnPassantWithNonPawn_ReturnsFalse()
    {
        var board = new Board("rnbqkb1r/ppp1pppp/8/8/3pPn2/8/PPPP1PPP/RNBQKBNR");
        var castlingAvailability = new CastlingAvailability("KQkq");
        var enPassantSquare = board["e3"];
        var sut = new Game(board, Color.Black,  castlingAvailability, enPassantSquare, 0, 5);
        
        var result = sut.MakeMove("f4", "e3");

        await Assert.That(result).IsFalse();
    }

    #endregion

    #region PawnPromotion

    [Test]
    public async Task MakeMove_WhenWhitePawnMovesToEighthRank_IsPromotedToGivenPiece()
    {
        var board = new Board("8/P7/8/8/8/8/K1k3p1/8");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.White, castlingAvailability, null, 0, 1);

        sut.MakeMove("a7", "a8", 'N');

        var actual = sut.Board["a8"].Piece!.Name;
        
        await Assert.That(actual).IsEqualTo('N');
    }
    
    [Test]
    public async Task MakeMove_WhenBlackPawnMovesToFirstRank_IsPromotedToGivenPiece()
    {
        var board = new Board("8/P7/8/8/8/8/K1k3p1/8");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.Black, castlingAvailability, null, 0, 1);

        sut.MakeMove("g2", "g1", 'b');

        var actual = sut.Board["g1"].Piece!.Name;
        await Assert.That(actual).IsEqualTo('b');
    }
    
    [Test]
    [Arguments('\0')] // Default char value
    [Arguments('B')] // Opposite color
    public async Task MakeMove_WhenPawnMovesToLastRankWithInvalidPromotionPieceChar_ThrowsInvalidPromotionException(char promotionPieceChar)
    {
        var board = new Board("8/P7/8/8/8/8/K1k3p1/8");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.Black, castlingAvailability, null, 0, 1);

        void Act() => sut.MakeMove("g2", "g1", promotionPieceChar);

        var exception = await Assert.That(Act).Throws<InvalidPromotionException>();
        await Assert.That(exception!.Message).IsEqualTo($"Invalid promotion character {promotionPieceChar} for color Black");
    }
    
    #endregion

    #region CastlingAvailability
    
    [Test]
    [Arguments(0, "e1", "g1", "f1", "g1", "h1")]
    [Arguments(0, "e1", "b1", "d1", "c1", "a1")]
    [Arguments(1, "e8", "h8", "f8", "g8", "h8")]
    [Arguments(1, "e8", "c8", "d8", "c8", "a8")]
    public async Task MakeMove_CastlingWhenValid_MovesKingAndRook(int turn, string kingSquare, string destinationSquare,
                                            string rookEndSquare, string kingEndSquare, string rookStartSquare)
    {
        var board = new Board("r3k2r/8/8/8/8/8/8/R3K2R");
        var castlingAvailability = new CastlingAvailability("KQkq");
        var sut = new Game(board, (Color)turn, castlingAvailability, null, 0, 1);

        sut.MakeMove(kingSquare, destinationSquare);

        await Assert.That(board[kingSquare].Piece).IsNull();
        await Assert.That(board[rookEndSquare].Piece).IsTypeOf<Rook>();
        await Assert.That(board[kingEndSquare].Piece).IsTypeOf<King>();
        await Assert.That(board[rookStartSquare].Piece).IsNull();
    }
    
    [Test]
    public async Task MakeMove_CastlingMove_UpdatesCastlingProperty()
    {
        var board = new Board("r3k2r/8/8/8/8/8/8/R3K2R");
        var castlingAvailability = new CastlingAvailability("Kkq");
        var sut = new Game(board, Color.Black, castlingAvailability, null, 0, 1);

        sut.MakeMove("e8", "a8");

        await Assert.That(sut.CastlingAvailability.ToString()).IsEqualTo("K");
    }

    [Test]
    [Arguments(0, "kq", "e1", "g1")]
    [Arguments(1, "k", "e8", "a8")]
    [Arguments(0, "-", "e1", "b1")]
    public async Task MakeMove_CastlingAfterKingOrRookHasMoved_ThrowsInvalidCastlingException
        (int turn, string castling, string from, string to)
    {
        var board = new Board("r3k2r/8/8/8/8/8/8/R3K2R");
        var castlingAvailability = new CastlingAvailability(castling);
        var sut = new Game(board, (Color)turn, castlingAvailability, null, 0, 1);

        void Act() => sut.MakeMove(from, to);

        var exception = await Assert.That(Act).Throws<InvalidCastlingException>();
        await Assert.That(exception!.Message).IsEqualTo($"Cannot castle from {from} to {to} because the king and/or rook have moved (CastlingAvailability: {castling}).");
    }

    [Test]
    [Arguments(0, "e1", "h1", "f")]
    [Arguments(0, "e1", "c1", "c")]
    [Arguments(1, "e8", "g8", "g")]
    [Arguments(1, "e8", "a8", "b")]
    public async Task MakeMove_CastlingWhenBlocked_ThrowsInvalidCastlingException(int turn, string from, string to, string blockedFile)
    {
        var board = new Board("rB2k1nr/8/8/8/8/8/8/R1N1KB1R");
        var castlingAvailability = new CastlingAvailability("KQkq");
        var sut = new Game(board, (Color)turn, castlingAvailability, null, 0, 1);
        
        void Act() => sut.MakeMove(from, to);

        var exception = await Assert.That(Act).Throws<InvalidCastlingException>();
        await Assert.That(exception!.Message).IsEqualTo($"Cannot castle from {from} to {to} because there is a piece blocking on file {blockedFile}");
    }

    #endregion
}
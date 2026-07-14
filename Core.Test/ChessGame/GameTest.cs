using Core.ChessBoard;
using Core.ChessGame;
using Core.Parsers;
using Core.Pieces;
using Core.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Test.ChessGame;

public class GameTest
{
    [Test]
    [Arguments(Color.White, "e7", "e5")]
    [Arguments(Color.Black, "e2", "e4")]
    public async Task MakeMove_MovingOpponentsPiece_ReturnsInvalidPiece(Color turn, string from, string to)
    {
        var castlingAvailability = new CastlingAvailability("KQkq");
        var sut = new Game(new Board(), turn, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove(from, to);

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.InvalidPiece);
    }

    [Test]
    public async Task MakeMove_WhenPieceIsNull_ReturnsInvalidPiece()
    {
        var castlingAvailability = new CastlingAvailability("KQkq");
        var sut = new Game(new Board(), Color.White, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove("a4", "a5");

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.InvalidPiece);
    }

    [Test]
    public async Task MakeMove_WithInvalidMove_ReturnsIllegalMove()
    {
        var castlingAvailability = new CastlingAvailability("KQkq");
        var sut = new Game(new Board(), Color.White, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove("e2", "b4");

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.IllegalMove);
    }

    [Test]
    public async Task MakeMove_WithValidMove_ReturnsSuccess()
    {
        var castlingAvailability = new CastlingAvailability("KQkq");
        var sut = new Game(new Board(), Color.White, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove("e2", "e4");

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.Success);
    }

    [Test]
    public async Task EndTurn_AfterWhiteMoves_TurnIsBlack()
    {
        var sut = new Game();

        var result = sut.MakeMove("e2", "e4").Game;

        await Assert.That(result.Turn).IsEqualTo(Color.Black);
    }

    [Test]
    public async Task EndTurn_AfterBlackMoves_TurnIsWhite()
    {
        var board = new Board("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR");
        var castlingAvailability = new CastlingAvailability("KQkq");
        var sut = new Game(board, Color.Black, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove("e7", "e5").Game;

        await Assert.That(result.Turn).IsEqualTo(Color.White);
    }

    [Test]
    public async Task EndTurn_AfterBlackMoves_FullMoveCountIncreases()
    {
        var board = new Board("r1bqkb1r/pppp1ppp/2n2n2/4p3/2B1P3/2N2N2/PPPP1PPP/R1BQK2R");
        var castlingAvailability = new CastlingAvailability("KQkq");
        var sut = new Game(board, Color.Black, castlingAvailability, null, 5, 4);

        var result = sut.MakeMove("f6", "e4").Game;

        await Assert.That(result.FullMoveCount).IsEqualTo(5);
    }

    [Test]
    public async Task UpdateHalfMoveCount_AfterPawnMove_ResetsHalfMoveCount()
    {
        var board = new Board("rnbqkbnr/ppp1pppp/8/8/8/2N2N2/PPPP1PPP/R1BQKB1R");
        var castlingAvailability = new CastlingAvailability("KQkq");
        var sut = new Game(board, Color.Black, castlingAvailability, null, 3, 4);

        var result = sut.MakeMove("e7", "e6").Game;

        await Assert.That(result.HalfMoveCount).IsEqualTo(0);
    }

    [Test]
    public async Task UpdateHalfMoveCount_AfterCapture_ResetsHalfMoveCount()
    {
        var board = new Board("r1bqkbnr/ppp2ppp/2n1p3/1B6/8/2N2N2/PPPP1PPP/R1BQK2R");
        var castlingAvailability = new CastlingAvailability("KQkq");
        var sut = new Game(board, Color.White, castlingAvailability, null, 2, 6);

        var result = sut.MakeMove("b5", "c6").Game;

        await Assert.That(result.HalfMoveCount).IsEqualTo(0);
    }

    [Test]
    public async Task UpdateHalfMoveCount_AfterPieceMoveWithoutCapture_IncrementsHalfMoveCount()
    {
        var board = new Board("r1bqkbnr/ppp2ppp/2n1p3/1B6/8/2N2N2/PPPP1PPP/R1BQ1RK1");
        var castlingAvailability = new CastlingAvailability("kq");
        var sut = new Game(board, Color.Black, castlingAvailability, null, 3, 6);

        var result = sut.MakeMove("g8", "f6").Game;

        await Assert.That(result.HalfMoveCount).IsEqualTo(4);
    }

    #region EnPassant

    [Test]
    public async Task MakeMove_WhenMoveIsEnPassant_ReturnsSuccess()
    {
        var board = new Board("rnbqkb1r/ppp1pppp/8/8/3pPn2/8/PPPP1PPP/RNBQKBNR");
        var castlingAvailability = new CastlingAvailability("KQkq");
        var enPassantSquare = board["e3"];
        var sut = new Game(board, Color.Black, castlingAvailability, enPassantSquare, 0, 5);

        var result = sut.MakeMove("d4", "e3");

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.Success);
    }

    [Test]
    public async Task MakeMove_WhenMoveIsEnPassant_ResetsEnPassantSquare()
    {
        var board = new Board("rnbqkb1r/ppp1pppp/8/8/3pPn2/8/PPPP1PPP/RNBQKBNR");
        var castlingAvailability = new CastlingAvailability("KQkq");
        var enPassantSquare = board["e3"];
        var sut = new Game(board, Color.Black, castlingAvailability, enPassantSquare, 0, 5);

        var result = sut.MakeMove("d4", "e3").Game;

        await Assert.That(result.EnPassant).IsNull();
    }

    [Test]
    public async Task MakeMove_WhenMoveIsEnPassant_RemovesCapturedPieceFromBoard()
    {
        var board = new Board("rnbqkb1r/ppp1pppp/8/8/3pPn2/8/PPPP1PPP/RNBQKBNR");
        var castlingAvailability = new CastlingAvailability("KQkq");
        var enPassantSquare = board["e3"];
        var sut = new Game(board, Color.Black, castlingAvailability, enPassantSquare, 0, 5);

        var result = sut.MakeMove("d4", "e3").Game;

        await Assert.That(result.Board["e4"].Piece).IsNull();
    }

    [Test]
    public async Task MakeMove_WhenMoveIsEnPassantWithNonPawn_ReturnsIllegalMove()
    {
        var board = new Board("rnbqkb1r/ppp1pppp/8/8/3pPn2/8/PPPP1PPP/RNBQKBNR");
        var castlingAvailability = new CastlingAvailability("KQkq");
        var enPassantSquare = board["e3"];
        var sut = new Game(board, Color.Black, castlingAvailability, enPassantSquare, 0, 5);

        var result = sut.MakeMove("f4", "e3");

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.IllegalMove);
    }

    #endregion

    #region PawnPromotion

    [Test]
    public async Task MakeMove_WhenWhitePawnMovesToEighthRank_IsPromotedToGivenPiece()
    {
        var board = new Board("8/P7/8/8/8/8/K1k3p1/8");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.White, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove("a7", "a8", 'N').Game;

        var actual = result.Board["a8"].Piece!.Name;

        await Assert.That(actual).IsEqualTo('N');
    }

    [Test]
    public async Task MakeMove_WhenBlackPawnMovesToFirstRank_IsPromotedToGivenPiece()
    {
        var board = new Board("8/P7/8/8/8/8/K1k3p1/8");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.Black, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove("g2", "g1", 'b').Game;

        var actual = result.Board["g1"].Piece!.Name;
        await Assert.That(actual).IsEqualTo('b');
    }

    [Test]
    [Arguments('\0')] // Default char value
    [Arguments('B')] // Opposite color
    public async Task MakeMove_WhenPawnMovesToLastRankWithInvalidPromotionPieceChar_ReturnsInvalidPromotion(char promotionPieceChar)
    {
        var board = new Board("8/P7/8/8/8/8/K1k3p1/8");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.Black, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove("g2", "g1", promotionPieceChar);

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.InvalidPromotion);
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

        var result = sut.MakeMove(kingSquare, destinationSquare).Game;

        await Assert.That(result.Board[kingSquare].Piece).IsNull();
        await Assert.That(result.Board[rookEndSquare].Piece).IsTypeOf<Rook>();
        await Assert.That(result.Board[kingEndSquare].Piece).IsTypeOf<King>();
        await Assert.That(result.Board[rookStartSquare].Piece).IsNull();
    }

    [Test]
    public async Task MakeMove_CastlingMove_UpdatesCastlingProperty()
    {
        var board = new Board("r3k2r/8/8/8/8/8/8/R3K2R");
        var castlingAvailability = new CastlingAvailability("Kkq");
        var sut = new Game(board, Color.Black, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove("e8", "a8").Game;

        await Assert.That(result.CastlingAvailability.ToString()).IsEqualTo("K");
    }

    [Test]
    [Arguments(0, "kq", "e1", "g1")]
    [Arguments(1, "k", "e8", "a8")]
    [Arguments(0, "-", "e1", "b1")]
    public async Task MakeMove_CastlingAfterKingOrRookHasMoved_ReturnsIllegalMove
        (int turn, string castling, string from, string to)
    {
        var board = new Board("r3k2r/8/8/8/8/8/8/R3K2R");
        var castlingAvailability = new CastlingAvailability(castling);
        var sut = new Game(board, (Color)turn, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove(from, to);

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.IllegalMove);
    }

    [Test]
    [Arguments(0, "e1", "h1")]
    [Arguments(0, "e1", "c1")]
    [Arguments(1, "e8", "g8")]
    [Arguments(1, "e8", "a8")]
    public async Task MakeMove_CastlingWhenBlocked_ReturnsIllegalMove(int turn, string from, string to)
    {
        var board = new Board("rB2k1nr/8/8/8/8/8/8/R1N1KB1R");
        var castlingAvailability = new CastlingAvailability("KQkq");
        var sut = new Game(board, (Color)turn, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove(from, to);

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.IllegalMove);
    }

    #endregion

    #region CheckDetection

    [Test]
    public async Task MakeMove_MoveThatLeavesKingInCheck_ReturnsIllegalMove()
    {
        // White rook on e4 is pinned to white king on e1 by black rook on e8
        var board = new Board("4r3/8/8/8/4R3/8/8/4K3");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.White, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove("e4", "d4");

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.IllegalMove);
    }

    [Test]
    public async Task MakeMove_MoveThatDoesNotLeaveKingInCheck_ReturnsSuccess()
    {
        // White rook on e4 is pinned to white king on e1 by black rook on e8
        // Moving it along the e-file is still legal
        var board = new Board("4r1k1/8/8/8/4R3/8/8/4K3");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.White, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove("e4", "e5");

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.Success);
    }

    [Test]
    public async Task MakeMove_KingMovesIntoCheck_ReturnsIllegalMove()
    {
        var board = new Board("4k3/8/8/8/8/8/2K5/r7");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.White, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove("c2", "c1");

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.IllegalMove);
    }

    [Test]
    public async Task MakeMove_EnPassantThatLeavesKingInCheck_ReturnsIllegalMove()
    {
        // Capturing d5 by en passant on d6 puts king in check from black rook on a5
        var board = new Board("8/8/8/r2pPK2/8/8/8/8");
        var castlingAvailability = new CastlingAvailability("-");
        var enPassantSquare = board["d6"];
        var sut = new Game(board, Color.White, castlingAvailability, enPassantSquare, 0, 10);

        var result = sut.MakeMove("e5", "d6");

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.IllegalMove);
    }

    [Test]
    [Arguments("e1", "g1")]
    [Arguments("e1", "c1")]
    public async Task MakeMove_CastlingThroughCheck_ReturnsIllegalMove(string from, string to)
    {
        var board = new Board("3rkr2/8/8/8/8/8/8/R3K2R");
        var castlingAvailability = new CastlingAvailability("KQ");
        var sut = new Game(board, Color.White, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove(from, to);

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.IllegalMove);
    }

    [Test]
    public async Task MakeMove_CastlingWhileInCheck_ReturnsIllegalMove()
    {
        var board = new Board("3kr3/8/8/8/8/8/8/R3K2R");
        var castlingAvailability = new CastlingAvailability("KQ");
        var sut = new Game(board, Color.White, castlingAvailability, null, 0, 1);

        var result = sut.MakeMove("e1", "g1");

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.IllegalMove);
    }

    #endregion

    #region MakeMove UCI

    [Test]
    public async Task MakeMove_Uci_ValidMove_ReturnsSuccess()
    {
        var game = new Game();

        var result = game.MakeMove("e2e4").MoveResult;

        await Assert.That(result).IsEqualTo(MoveResult.Success);
    }

    [Test]
    public async Task MakeMove_Uci_ValidMove_UpdatesBoard()
    {
        var game = new Game();

        game = game.MakeMove("e2e4").Game;

        await Assert.That(game.Board["e4"].Piece).IsTypeOf<Pawn>();
        await Assert.That(game.Board["e2"].Piece).IsNull();
    }

    [Test]
    public async Task MakeMove_Uci_InvalidMove_ReturnsIllegalMove()
    {
        var game = new Game();

        var result = game.MakeMove("e2b4").MoveResult;

        await Assert.That(result).IsEqualTo(MoveResult.IllegalMove);
    }

    [Test]
    public async Task MakeMove_Uci_WrongLength_ReturnsIllegalMove()
    {
        var game = new Game();

        var result = game.MakeMove("e2->e4").MoveResult;

        await Assert.That(result).IsEqualTo(MoveResult.IllegalMove);
    }

    [Test]
    [Arguments("a7a8q", typeof(Queen))]
    [Arguments("a7a8r", typeof(Rook))]
    [Arguments("a7a8b", typeof(Bishop))]
    [Arguments("a7a8n", typeof(Knight))]
    public async Task MakeMove_Uci_WhitePromotion_PlacesCorrectPiece(string uci, Type expectedType)
    {
        var game = FenParser.Parse("4k3/P7/8/8/8/8/8/4K3 w - - 0 1");

        game = game.MakeMove(uci).Game;

        await Assert.That(game.Board["a8"].Piece).IsOfType(expectedType);
        await Assert.That(game.Board["a8"].Piece!.Color).IsEqualTo(Color.White);
    }

    [Test]
    [Arguments("a2a1q", typeof(Queen))]
    [Arguments("a2a1r", typeof(Rook))]
    [Arguments("a2a1b", typeof(Bishop))]
    [Arguments("a2a1n", typeof(Knight))]
    public async Task MakeMove_Uci_BlackPromotion_PlacesCorrectPiece(string uci, Type expectedType)
    {
        var game = FenParser.Parse("4K3/8/8/8/8/8/p7/4k3 b - - 0 1");

        game = game.MakeMove(uci).Game;

        await Assert.That(game.Board["a1"].Piece).IsOfType(expectedType);
        await Assert.That(game.Board["a1"].Piece!.Color).IsEqualTo(Color.Black);
    }

    [Test]
    public async Task MakeMove_Uci_Castling_KingsideWhite()
    {
        var game = FenParser.Parse("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");

        game = game.MakeMove("e1g1").Game;

        await Assert.That(game.Board["g1"].Piece).IsTypeOf<King>();
        await Assert.That(game.Board["f1"].Piece).IsTypeOf<Rook>();
        await Assert.That(game.Board["e1"].Piece).IsNull();
        await Assert.That(game.Board["h1"].Piece).IsNull();
    }

    [Test]
    public async Task MakeMove_Uci_MultipleMoves_AlternatesTurns()
    {
        var game = new Game();

        game = game.MakeMove("e2e4").Game;
        game = game.MakeMove("e7e5").Game;
        game = game.MakeMove("g1f3").Game;

        await Assert.That(game.Turn).IsEqualTo(Color.Black);
        await Assert.That(game.Board["f3"].Piece).IsTypeOf<Knight>();
        await Assert.That(game.Board["e4"].Piece).IsTypeOf<Pawn>();
        await Assert.That(game.Board["e5"].Piece).IsTypeOf<Pawn>();
    }

    #endregion

    #region GameResult

    [Test]
    public async Task Result_AtStartOfGame_IsInProgress()
    {
        var sut = new Game();

        await Assert.That(sut.GameResult).IsEqualTo(GameResult.InProgress);
    }

    [Test]
    public async Task Result_AfterNonTerminatingMove_IsInProgress()
    {
        var sut = new Game();

        sut.MakeMove("e2", "e4");

        await Assert.That(sut.GameResult).IsEqualTo(GameResult.InProgress);
    }

    [Test]
    public async Task Result_AfterFoolsMate_IsCheckmate()
    {
        var sut = new Game();
        sut = sut.MakeMove("f2", "f3").Game;
        sut = sut.MakeMove("e7", "e5").Game;
        sut = sut.MakeMove("g2", "g4").Game;
        sut = sut.MakeMove("d8", "h4").Game;

        await Assert.That(sut.GameResult).IsEqualTo(GameResult.Checkmate);
    }

    [Test]
    public async Task Result_AfterCheckmate_TurnIsLosingColor()
    {
        // After fool's mate white is checkmated. It is white's turn, but they have no legal moves
        var sut = new Game();
        sut.MakeMove("f2", "f3");
        sut.MakeMove("e7", "e5");
        sut.MakeMove("g2", "g4");
        sut.MakeMove("d8", "h4");

        await Assert.That(sut.Turn).IsEqualTo(Color.White);
    }

    [Test]
    public async Task MakeMove_AfterCheckmate_ReturnsGameAlreadyOver()
    {
        var sut = new Game();
        sut = sut.MakeMove("f2", "f3").Game;
        sut = sut.MakeMove("e7", "e5").Game;
        sut = sut.MakeMove("g2", "g4").Game;
        sut = sut.MakeMove("d8", "h4").Game;

        var result = sut.MakeMove("e2", "e4");

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.GameAlreadyOver);
    }

    [Test]
    public async Task Result_AfterStalemate_IsStalemate()
    {
        var board = new Board("k7/8/1QK5/8/8/8/8/8");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.Black, castlingAvailability, null, 0, 1);

        await Assert.That(sut.GameResult).IsEqualTo(GameResult.Stalemate);
    }

    [Test]
    public async Task GetLegalMoves_InStartingPosition_Returns20Moves()
    {
        // 16 pawn moves + 4 knight moves
        var sut = new Game();

        int count = sut.GetLegalMoves(Color.White).Count();

        await Assert.That(count).IsEqualTo(20);
    }

    [Test]
    public async Task GetLegalMoves_WhenKingIsInCheckWithNoBlockers_OnlyKingMovesAreReturned()
    {
        // White king on e1 is in check from black rook on e8, only legal moves resolve the check
        var board = new Board("4r3/8/8/PPk5/2P5/6PP/5P2/4K3");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.White, castlingAvailability, null, 0, 1);

        var legalMoves = sut.GetLegalMoves(Color.White).ToList();

        await Assert.That(legalMoves).IsNotEmpty();
        await Assert.That(legalMoves.All(m => m.from.Piece is King)).IsTrue();
    }

    [Test]
    public async Task GetLegalMoves_WhenKingInDoubleCheck_OnlyKingMovesAreReturned()
    {
        // King on e1 in double check from rook on e8 and bishop on h4
        // White rook on d3 could block the e-file or e1-h4 diagonal but can't block both
        // at the same time, meaning the king must move
        var board = new Board("4r3/8/2k5/8/7b/3R4/7N/4K3");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.White, castlingAvailability, null, 0, 1);

        var legalMoves = sut.GetLegalMoves(Color.White).ToList();

        await Assert.That(legalMoves).IsNotEmpty();
        await Assert.That(legalMoves.All(m => m.from.Piece is King)).IsTrue();
    }

    [Test]
    public async Task MakeMove_EnPassantThatCapturesCheckingPawn_ReturnsSuccess()
    {
        // Black pawn just moved d7-d5, now attacks e4 where the white king stands
        // White pawn on e5 can capture en passant on d6, removing the checking pawn from d5
        var board = new Board("4k3/8/8/3pP3/4K3/8/8/8");
        var castlingAvailability = new CastlingAvailability("-");
        Square enPassantSquare = board["d6"];
        var sut = new Game(board, Color.White, castlingAvailability, enPassantSquare, 0, 1);

        var result = sut.MakeMove("e5", "d6");

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.Success);
    }

    [Test]
    public async Task MakeMove_FailedMoveReturnsOriginalGame()
    {
        var sut = new Game();
        var result = sut.MakeMove("e2", "d5");

        await Assert.That(result.Game).IsSameReferenceAs(sut);
    }

    #endregion

    #region Draw Conditions

    [Test]
    public async Task Result_After49MovesWithoutCaptureOrPawnMove_IsInProgress()
    {
        var board = new Board("8/p7/4k3/8/8/4K3/8/8");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.White, castlingAvailability, null, 99, 50);

        await Assert.That(sut.GameResult).IsEqualTo(GameResult.InProgress);
    }

    [Test]
    public async Task Result_After75MovesWithoutCaptureOrPawnMove_IsDrawBySeventyFiveMoveRule()
    {
        var board = new Board("8/p7/4k3/8/8/4K3/8/8");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.White, castlingAvailability, null, 150, 76);

        await Assert.That(sut.GameResult).IsEqualTo(GameResult.DrawBySeventyFiveMoveRule);
    }


    [Test]
    public async Task Result_SeventyFiveMoveRuleCounterResetsAfterPawnMove_IsInProgress()
    {
        var board = new Board("8/p7/4k3/8/8/4K3/8/8");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.Black, castlingAvailability, null, 149, 75);

        sut = sut.MakeMove("a7", "a5").Game;

        await Assert.That(sut.GameResult).IsEqualTo(GameResult.InProgress);
        await Assert.That(sut.HalfMoveCount).IsEqualTo(0);
    }

    [Test]
    public async Task Result_SeventyFiveMoveRuleCounterResetsAfterCapture_IsInProgress()
    {
        var board = new Board("3k4/8/8/8/8/4r3/3K4/4R3");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.White, castlingAvailability, null, 149, 50);

        var result = sut.MakeMove("e1", "e3").Game;

        await Assert.That(result.GameResult).IsEqualTo(GameResult.InProgress);
        await Assert.That(result.HalfMoveCount).IsEqualTo(0);
    }

    [Test]
    public async Task Result_KingVsKing_IsDrawByInsufficientMaterial()
    {
        var board = new Board("4k3/8/8/8/8/8/8/4K3");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.White, castlingAvailability, null, 0, 1);

        await Assert.That(sut.GameResult).IsEqualTo(GameResult.DrawByInsufficientMaterial);
    }

    [Test]
    [Arguments("4k3/8/8/8/8/8/8/4KN2", Color.White)]
    [Arguments("4kn2/8/8/8/8/8/8/4K3", Color.Black)]
    public async Task Result_KingAndKnightVsKing_IsDrawByInsufficientMaterial(string boardFen, Color turn)
    {
        var board = new Board(boardFen);
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, turn, castlingAvailability, null, 0, 37);

        await Assert.That(sut.GameResult).IsEqualTo(GameResult.DrawByInsufficientMaterial);
    }

    [Test]
    [Arguments("4k3/8/8/8/8/8/8/4KB2", Color.White)]
    [Arguments("4kb2/8/8/8/8/8/8/4K3", Color.Black)]
    public async Task Result_KingAndBishopVsKing_IsDrawByInsufficientMaterial(string boardFen, Color turn)
    {
        var board = new Board(boardFen);
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, turn, castlingAvailability, null, 0, 1);

        await Assert.That(sut.GameResult).IsEqualTo(GameResult.DrawByInsufficientMaterial);
    }

    [Test]
    public async Task Result_KingAndBishopVsKingAndBishop_SameColorSquares_IsDrawByInsufficientMaterial()
    {
        var board = new Board("7b/8/8/8/8/8/8/BK5k");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.White, castlingAvailability, null, 0, 1);

        await Assert.That(sut.GameResult).IsEqualTo(GameResult.DrawByInsufficientMaterial);
    }

    [Test]
    public async Task Result_KingAndBishopVsKingAndBishop_OppositeColorSquares_IsInProgress()
    {
        // White bishop on light square (a1), black bishop on dark square (a8): (0+7)%2=1
        var board = new Board("b7/8/8/8/8/8/8/BK5k");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.Black, castlingAvailability, null, 0, 1);

        await Assert.That(sut.GameResult).IsEqualTo(GameResult.InProgress);
    }

    [Test]
    public async Task Result_KingAndQueenVsKing_IsInProgress()
    {
        var board = new Board("4k3/8/8/8/8/8/8/4KQ2");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.White, castlingAvailability, null, 0, 1);

        await Assert.That(sut.GameResult).IsEqualTo(GameResult.InProgress);
    }

    [Test]
    public async Task Result_KingAndPawnVsKing_IsInProgress()
    {
        var board = new Board("4k3/8/8/8/8/8/4P3/4K3");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.White, castlingAvailability, null, 0, 1);

        await Assert.That(sut.GameResult).IsEqualTo(GameResult.InProgress);
    }

    [Test]
    public async Task Result_PositionRepeatedFiveTimes_IsDrawByFivefoldRepetition()
    {
        var board = new Board("7k/R7/7K/8/8/8/8/8");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.White, castlingAvailability, null, 0, 1);

        // Repeat position 4 more times (5 total)
        sut = sut.MakeMove("h6", "g6").Game;
        sut = sut.MakeMove("h8", "g8").Game;
        sut = sut.MakeMove("g6", "h6").Game;
        sut = sut.MakeMove("g8", "h8").Game; // 2nd occurence

        sut = sut.MakeMove("h6", "g6").Game;
        sut = sut.MakeMove("h8", "g8").Game;
        sut = sut.MakeMove("g6", "h6").Game;
        sut = sut.MakeMove("g8", "h8").Game; // 3nd occurence

        sut = sut.MakeMove("h6", "g6").Game;
        sut = sut.MakeMove("h8", "g8").Game;
        sut = sut.MakeMove("g6", "h6").Game;
        sut = sut.MakeMove("g8", "h8").Game; // 4th occurence

        sut = sut.MakeMove("h6", "g6").Game;
        sut = sut.MakeMove("h8", "g8").Game;
        sut = sut.MakeMove("g6", "h6").Game;
        sut = sut.MakeMove("g8", "h8").Game; // 5th occurence

        await Assert.That(sut.GameResult).IsEqualTo(GameResult.DrawByFivefoldRepetition);
    }

    [Test]
    public async Task Result_PositionRepeatedFourTimes_IsInProgress()
    {
        var board = new Board("7k/R7/7K/8/8/8/8/8");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.White, castlingAvailability, null, 0, 1);

        // Repeat position 3 more times (4 total)
        sut = sut.MakeMove("h6", "g6").Game;
        sut = sut.MakeMove("h8", "g8").Game;
        sut = sut.MakeMove("g6", "h6").Game;
        sut = sut.MakeMove("g7", "h8").Game; // 2nd occurence

        sut = sut.MakeMove("h6", "g6").Game;
        sut = sut.MakeMove("h8", "g8").Game;
        sut = sut.MakeMove("g6", "h6").Game;
        sut = sut.MakeMove("g7", "h8").Game; // 3nd occurence

        sut = sut.MakeMove("h6", "g6").Game;
        sut = sut.MakeMove("h8", "g8").Game;
        sut = sut.MakeMove("g6", "h6").Game;
        sut = sut.MakeMove("g7", "h8").Game; // 4th occurence

        await Assert.That(sut.GameResult).IsEqualTo(GameResult.InProgress);
    }

    [Test]
    public async Task Result_PositionDiffersByCastlingRights_NotRepetition()
    {
        var board = new Board("b3k2r/B7/8/8/8/8/8/4K2R");
        var castlingAvailability = new CastlingAvailability("Kk");
        var sut = new Game(board, Color.White, castlingAvailability, null, 0, 1);

        // Shuffle white and black bishop.
        // Then move the white rook back and forth to create the same board position but with different castling rights
        sut.MakeMove("a7", "b8");
        sut.MakeMove("a8", "b7");
        sut.MakeMove("b8", "a7");
        sut.MakeMove("b7", "a8"); // 2nd occurence

        sut.MakeMove("a7", "b8");
        sut.MakeMove("a8", "b7");
        sut.MakeMove("b8", "a7");
        sut.MakeMove("b7", "a8"); // 3rd occurence

        sut.MakeMove("a7", "b8");
        sut.MakeMove("a8", "b7");
        sut.MakeMove("b8", "a7");
        sut.MakeMove("b7", "a8"); // 4th occurence

        sut.MakeMove("h1", "h2"); // Shuffle white rook to change castling rights
        sut.MakeMove("a8", "b7");
        sut.MakeMove("h2", "h1");
        sut.MakeMove("b7", "a8"); // 5th occurence but with different castling rights

        await Assert.That(sut.GameResult).IsEqualTo(GameResult.InProgress);
    }

    [Test]
    public async Task Result_PositionDiffersByTurn_NotRepetition()
    {
        var board = new Board("7k/R7/7K/8/8/8/8/8");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.White, castlingAvailability, null, 0, 1);

        // Repeat position 4 more times (5 total)
        sut = sut.MakeMove("a7", "c7").Game;
        sut = sut.MakeMove("h8", "g8").Game;
        sut = sut.MakeMove("c7", "b7").Game;
        sut = sut.MakeMove("g8", "h8").Game;
        sut = sut.MakeMove("b7", "a7").Game; // 2nd occurence of position but with different turn

        sut = sut.MakeMove("h8", "g8").Game;
        sut = sut.MakeMove("h6", "g6").Game;
        sut = sut.MakeMove("g8", "h8").Game;
        sut = sut.MakeMove("g6", "h6").Game; // 3nd occurence (2nd of different turn)

        sut = sut.MakeMove("h8", "g8").Game;
        sut = sut.MakeMove("h6", "g6").Game;
        sut = sut.MakeMove("g8", "h8").Game;
        sut = sut.MakeMove("g6", "h6").Game; // 4th occurence (3rd of different turn)

        sut = sut.MakeMove("h8", "g8").Game;
        sut = sut.MakeMove("h6", "g6").Game;
        sut = sut.MakeMove("g8", "h8").Game;
        sut = sut.MakeMove("g6", "h6").Game; // 5th occurence (4th of different turn)

        await Assert.That(sut.GameResult).IsEqualTo(GameResult.InProgress);
    }

    [Test]
    public async Task Result_PositionDiffersByEnPassant_NotRepetition()
    {
        var board = new Board("3k4/8/3K4/6Pp/8/8/8/8");
        var castlingAvailability = new CastlingAvailability("-");
        var sut = new Game(board, Color.White, castlingAvailability, board["h6"], 0, 1);

        // Repeat position 4 more times (5 total)
        sut = sut.MakeMove("d6", "c6").Game;
        sut = sut.MakeMove("d8", "c8").Game;
        sut = sut.MakeMove("c6", "d6").Game;
        sut = sut.MakeMove("c8", "d8").Game; // 2nd occurence of position but without EnPassant square

        sut = sut.MakeMove("d6", "c6").Game;
        sut = sut.MakeMove("d8", "c8").Game;
        sut = sut.MakeMove("c6", "d6").Game;
        sut = sut.MakeMove("c8", "d8").Game; // 3nd occurence (2nd without EnPassant)

        sut = sut.MakeMove("d6", "c6").Game;
        sut = sut.MakeMove("d8", "c8").Game;
        sut = sut.MakeMove("c6", "d6").Game;
        sut = sut.MakeMove("c8", "d8").Game; // 4th occurence (3rd without EnPassant)

        sut = sut.MakeMove("d6", "c6").Game;
        sut = sut.MakeMove("d8", "c8").Game;
        sut = sut.MakeMove("c6", "d6").Game;
        sut = sut.MakeMove("c8", "d8").Game; // 5th occurence (4th without EnPassant)

        await Assert.That(sut.GameResult).IsEqualTo(GameResult.InProgress);
    }

    #endregion

    #region CurrentBranchLength

    [Test]
    public async Task CurrentBranchLength_AtStartOfGame_IsZero()
    {
        var sut = new Game();

        await Assert.That(sut.CurrentBranchLength).IsZero();
    }

    [Test]
    public async Task CurrentBranchLength_AfterSuccessfulMove_IsOne()
    {
        var sut = new Game();

        var game = sut.MakeMove("e2", "e4").Game;

        await Assert.That(game.CurrentBranchLength).IsEqualTo(1);
    }

    [Test]
    public async Task CurrentBranchLengthy_AfterFailedMove_IsUnchanged()
    {
        var sut = new Game();

        var result = sut.MakeMove("e2", "b4");

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.IllegalMove);
        await Assert.That(result.Game.CurrentBranchLength).IsZero();
    }

    [Test]
    public async Task CurrentBranchLength_AfterThreeMoves_IsThree()
    {
        var sut = new Game();
        sut = sut.MakeMove("e2", "e4").Game;
        sut = sut.MakeMove("e7", "e5").Game;
        sut = sut.MakeMove("g1", "f3").Game;

        await Assert.That(sut.CurrentBranchLength).IsEqualTo(3);
    }

    #endregion

    #region CurrentMoveIndex

    [Test]
    public async Task CurrentMoveIndex_AtStartOfGame_IsZero()
    {
        var sut = new Game();

        await Assert.That(sut.CurrentMoveIndex).IsEqualTo(0);
    }

    [Test]
    public async Task CurrentMoveIndex_AfterOneMove_IsOne()
    {
        var sut = new Game();
        sut = sut.MakeMove("e2", "e4").Game;

        await Assert.That(sut.CurrentMoveIndex).IsEqualTo(1);
    }

    [Test]
    public async Task CurrentMoveIndex_AfterThreeMoves_IsThree()
    {
        var sut = new Game();
        sut = sut.MakeMove("e2", "e4").Game;
        sut = sut.MakeMove("e7", "e5").Game;
        sut = sut.MakeMove("g1", "f3").Game;

        await Assert.That(sut.CurrentMoveIndex).IsEqualTo(3);
    }

    #endregion

    #region GoToMove

    [Test]
    public async Task GoToMove_ToCurrentIndex_ReturnsSameInstance()
    {
        var sut = new Game();
        sut = sut.MakeMove("e2", "e4").Game;

        var result = sut.GoToMove(1);

        await Assert.That(result).IsSameReferenceAs(sut);
    }

    [Test]
    public async Task GoToMove_ToStartPosition_ReturnsInitialBoardState()
    {
        var sut = new Game();
        sut = sut.MakeMove("e2", "e4").Game;
        sut = sut.MakeMove("e7", "e5").Game;

        var result = sut.GoToMove(0);

        await Assert.That(result.CurrentMoveIndex).IsEqualTo(0);
        await Assert.That(result.Turn).IsEqualTo(Color.White);
        await Assert.That(result.Board["e2"].Piece).IsNotNull();
        await Assert.That(result.Board["e4"].Piece).IsNull();
    }

    [Test]
    public async Task GoToMove_ToMiddleOfGame_ReconstructsCorrectPosition()
    {
        var sut = new Game();
        sut = sut.MakeMove("e2", "e4").Game;
        sut = sut.MakeMove("e7", "e5").Game;
        sut = sut.MakeMove("g1", "f3").Game;

        var result = sut.GoToMove(1);

        await Assert.That(result.CurrentMoveIndex).IsEqualTo(1);
        await Assert.That(result.Turn).IsEqualTo(Color.Black);
        await Assert.That(result.Board["e4"].Piece).IsNotNull();
        await Assert.That(result.Board["e7"].Piece).IsNotNull();
        await Assert.That(result.Board["e5"].Piece).IsNull();
    }

    [Test]
    public async Task GoToMove_PreservesCurrentBranchLength()
    {
        var sut = new Game();
        sut = sut.MakeMove("e2", "e4").Game;
        sut = sut.MakeMove("e7", "e5").Game;
        sut = sut.MakeMove("g1", "f3").Game;

        var result = sut.GoToMove(1);

        await Assert.That(result.CurrentBranchLength).IsEqualTo(3);
    }

    [Test]
    public async Task GoToMove_NegativeIndex_ThrowsArgumentOutOfRangeException()
    {
        var sut = new Game();
        sut = sut.MakeMove("e2", "e4").Game;

        Game action() => sut.GoToMove(-1);

        await Assert.That(action).ThrowsExactly<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task GoToMove_IndexBeyondHistory_ThrowsArgumentOutOfRangeException()
    {
        var sut = new Game();
        sut = sut.MakeMove("e2", "e4").Game;

        Game action() => sut.GoToMove(5);

        await Assert.That(action).ThrowsExactly<ArgumentOutOfRangeException>();
    }

    #endregion

    #region GoToNextMove / GoToPreviousMove

    [Test]
    public async Task GoToPreviousMove_AfterTwoMoves_ReturnsPositionAfterFirstMove()
    {
        var sut = new Game();
        sut = sut.MakeMove("e2", "e4").Game;
        sut = sut.MakeMove("e7", "e5").Game;

        var result = sut.GoToPreviousMove();

        await Assert.That(result.CurrentMoveIndex).IsEqualTo(1);
        await Assert.That(result.Turn).IsEqualTo(Color.Black);
    }

    [Test]
    public async Task GoToNextMove_AfterNavigatingBack_RestoresNextPosition()
    {
        var sut = new Game();
        sut = sut.MakeMove("e2", "e4").Game;
        sut = sut.MakeMove("e7", "e5").Game;
        sut = sut.MakeMove("g1", "f3").Game;

        var navigatedBack = sut.GoToMove(1);
        var navigatedForward = navigatedBack.GoToNextMove();

        await Assert.That(navigatedForward.CurrentMoveIndex).IsEqualTo(2);
        await Assert.That(navigatedForward.Turn).IsEqualTo(Color.White);
        await Assert.That(navigatedForward.Board["e5"].Piece).IsNotNull();
        await Assert.That(navigatedForward.Board["f3"].Piece).IsNull();
    }

    [Test]
    public async Task GoToPreviousMove_AtStartOfGame_ThrowsArgumentOutOfRangeException()
    {
        var sut = new Game();

        Game action() => sut.GoToPreviousMove();

        await Assert.That(action).ThrowsExactly<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task GoToNextMove_AtEndOfHistory_ThrowsArgumentOutOfRangeException()
    {
        var sut = new Game();
        sut = sut.MakeMove("e2", "e4").Game;

        Game action() => sut.GoToNextMove();

        await Assert.That(action).ThrowsExactly<ArgumentOutOfRangeException>();
    }

    #endregion

    #region Immutability

    [Test]
    public async Task MakeMove_DoesNotMutateOriginalGame()
    {
        var original = new Game();
        original.MakeMove("e2", "e4");

        await Assert.That(original.Board["e2"].Piece).IsNotNull();
        await Assert.That(original.Board["e4"].Piece).IsNull();
        await Assert.That(original.Turn).IsEqualTo(Color.White);
        await Assert.That(original.CurrentMoveIndex).IsEqualTo(0);
    }

    [Test]
    public async Task MakeMove_ReturnsNewGameInstance()
    {
        var original = new Game();
        var result = original.MakeMove("e2", "e4");

        await Assert.That(result.Game).IsNotSameReferenceAs(original);
    }

    [Test]
    public async Task GoToMove_DoesNotMutateOriginalGame()
    {
        var sut = new Game();
        sut = sut.MakeMove("e2", "e4").Game;
        sut = sut.MakeMove("e7", "e5").Game;

        var navigated = sut.GoToMove(0);

        await Assert.That(sut.CurrentMoveIndex).IsEqualTo(2);
        await Assert.That(navigated.CurrentMoveIndex).IsEqualTo(0);
    }

    [Test]
    public async Task GoToMove_ToStartPosition_FromFenGame_ReturnsCustomInitialPosition()
    {
        var sut = FenParser.Parse("4k3/8/R3K3/8/8/8/8/8 w - - 0 1");
        sut = sut.MakeMove("a6", "a8").Game;

        var result = sut.GoToMove(0);

        await Assert.That(result.Board["a6"].Piece).IsNotNull();
        await Assert.That(result.Board["a8"].Piece).IsNull();
    }


    [Test]
    public async Task GoToMove_ToStartPosition_FromFenGame_PreservesInitialTurn()
    {
        var sut = FenParser.Parse("4k3/8/8/8/8/8/8/4K2R b K - 0 1");
        sut = sut.MakeMove("e8", "d8").Game;

        var result = sut.GoToMove(0);

        await Assert.That(result.Turn).IsEqualTo(Color.Black);
    }

    #endregion
}

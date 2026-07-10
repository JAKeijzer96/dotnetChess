using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ChessGame;
using Core.Exceptions;
using Core.Parsers;
using Core.Pieces;
using Core.Shared;

namespace Core.Test.Parsers;

public class PgnParserParseTest
{
    private static string PgnFromMovetext(string moveText, string result = "*", string? fen = null)
    {
        string newLine = Environment.NewLine;
        var baseTags = $"[Event \"?\"{newLine}[Site \"?\"{newLine}[Date \"????.??.??\"{newLine}[Round \"?\"{newLine}[White \"?\"{newLine}[Black \"?\"{newLine}";
        var resultTag = $"[Result \"{result}\"]";
        var fenTag = $"[FEN \"{fen}\"]";
        return baseTags + resultTag + (fen is not null ? $"{newLine}{fenTag}" : "") + $"{newLine}{newLine}" + moveText;
    }

    #region Error handling

    [Test]
    public async Task Parse_NullInput_ThrowsArgumentNullException()
    {
        static void Act() => PgnParser.Parse(null!);
        await Assert.That(Act).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task Parse_InvalidSan_ThrowsInvalidPgnException()
    {
        var pgn = PgnFromMovetext("1. Ni2 *");
        void Act() => PgnParser.Parse(pgn);

        await Assert.That(Act).ThrowsExactly<InvalidPgnException>();
    }

    [Test]
    public async Task Parse_UnmatchedCloseParen_ThrowsInvalidPgnException()
    {
        var pgn = PgnFromMovetext("1. e4 ) *");
        void Act() => PgnParser.Parse(pgn);

        await Assert.That(Act).ThrowsExactly<InvalidPgnException>();
    }

    [Test]
    public async Task Parse_VariationOpenAtMoveZero_ThrowsInvalidPgnException()
    {
        var pgn = PgnFromMovetext("( 1. e4 ) *");
        void Act() => PgnParser.Parse(pgn);

        await Assert.That(Act).ThrowsExactly<InvalidPgnException>();
    }

    [Test]
    [Arguments("Event")]
    [Arguments("Site")]
    [Arguments("Date")]
    [Arguments("Round")]
    [Arguments("White")]
    [Arguments("Black")]
    [Arguments("Result")]
    public async Task Parse_MissingRequiredTag_ThrowsInvalidPgnException(string missingTag)
    {
        var tags = new Dictionary<string, string>
        {
            ["Event"] = "?",
            ["Site"] = "?",
            ["Date"] = "????.??.??",
            ["Round"] = "?",
            ["White"] = "?",
            ["Black"] = "?",
            ["Result"] = "*",
        };
        tags.Remove(missingTag);

        string pgn = string.Join("\r\n", tags.Select(t => $"[{t.Key} \"{t.Value}\"]")) + "\r\n\r\n*";
        void Act() => PgnParser.Parse(pgn);

        await Assert.That(Act).ThrowsExactly<InvalidPgnException>();
    }

    [Test]
    [Arguments("1-0", "*")]
    [Arguments("0-1", "1-0")]
    [Arguments("1/2-1/2", "1-0")]
    [Arguments("*", "1/2-1/2")]
    public async Task Parse_MovetextResultMismatchesResultTag_ThrowsInvalidPgnException(string resultTag, string movextextResult)
    {
        string pgn = PgnFromMovetext(movextextResult, resultTag);
        void Act() => PgnParser.Parse(pgn);

        await Assert.That(Act).ThrowsExactly<InvalidPgnException>();
    }

    [Test]
    [Arguments("1-0")]
    [Arguments("0-1")]
    [Arguments("1/2-1/2")]
    [Arguments("*")]
    public async Task Parse_MovetextResultMatchesResultTag_DoesNotThrow(string result)
    {
        string pgn = PgnFromMovetext(result, result);
        void Act() => PgnParser.Parse(pgn);

        await Assert.That(Act).ThrowsNothing();
    }

    #endregion

    #region Comment and annotation stripping

    [Test]
    public async Task Parse_WithBraceComment_ParsesMoves()
    {
        var pgn = PgnFromMovetext("1. e4 { This is a comment } e5 *");

        var game = PgnParser.Parse(pgn);

        var e4Piece = game.Board["e4"].Piece!;
        var e5Piece = game.Board["e5"].Piece!;
        await Assert.That(e4Piece).IsTypeOf<Pawn>();
        await Assert.That(e4Piece.Color).IsEqualTo(Color.White);
        await Assert.That(e5Piece).IsTypeOf<Pawn>();
        await Assert.That(e5Piece.Color).IsEqualTo(Color.Black);
    }

    [Test]
    public async Task Parse_WithSemicolonComment_ParsesMoves()
    {
        var pgn = PgnFromMovetext("1. e4 e5 ; this is a line comment\n2. Nf3 *");

        var game = PgnParser.Parse(pgn);

        var e4Piece = game.Board["e4"].Piece!;
        var e5Piece = game.Board["e5"].Piece!;
        var f3Piece = game.Board["f3"].Piece!;
        await Assert.That(e4Piece).IsTypeOf<Pawn>();
        await Assert.That(e4Piece.Color).IsEqualTo(Color.White);
        await Assert.That(e5Piece).IsTypeOf<Pawn>();
        await Assert.That(e5Piece.Color).IsEqualTo(Color.Black);
        await Assert.That(f3Piece).IsTypeOf<Knight>();
        await Assert.That(f3Piece.Color).IsEqualTo(Color.White);
        await Assert.That(game.CurrentMoveIndex).IsEqualTo(3);
    }

    [Test]
    public async Task Parse_WithNag_ParsesMoves()
    {
        var pgn = PgnFromMovetext("1. e4 $1 e5 $2 *");

        var game = PgnParser.Parse(pgn);

        var e4Piece = game.Board["e4"].Piece!;
        var e5Piece = game.Board["e5"].Piece!;
        await Assert.That(e4Piece).IsTypeOf<Pawn>();
        await Assert.That(e4Piece.Color).IsEqualTo(Color.White);
        await Assert.That(e5Piece).IsTypeOf<Pawn>();
        await Assert.That(e5Piece.Color).IsEqualTo(Color.Black);
        await Assert.That(game.CurrentMoveIndex).IsEqualTo(2);
    }

    #endregion



    #region FEN tag handling

    [Test]
    public async Task Parse_WithFenTag_AppliesFenToInitialBoardState()
    {
        // Only a rook and kings on the board
        var pgn = PgnFromMovetext("*", fen: "4k3/8/8/8/8/8/8/R3K3 w Q - 0 1");

        var game = PgnParser.Parse(pgn);

        await Assert.That(game.Board["a1"].Piece).IsTypeOf<Rook>();
        await Assert.That(game.Board["a1"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(game.Board["e1"].Piece).IsTypeOf<King>();
        await Assert.That(game.Board["e8"].Piece).IsTypeOf<King>();
        await Assert.That(game.Board["e2"].Piece).IsNull();
        await Assert.That(game.Board["g8"].Piece).IsNull();
        await Assert.That(game.Turn).IsEqualTo(Color.White);
    }

    [Test]
    public async Task Parse_WithoutFenTag_UsesStandardStartingPosition()
    {
        var pgn = PgnFromMovetext("*");

        var game = PgnParser.Parse(pgn);

        await Assert.That(game.Board["e1"].Piece).IsTypeOf<King>();
        await Assert.That(game.Board["e8"].Piece).IsTypeOf<King>();
        await Assert.That(game.Board["d1"].Piece).IsTypeOf<Queen>();
        await Assert.That(game.Board["a1"].Piece).IsTypeOf<Rook>();
        await Assert.That(game.Board["e4"].Piece).IsNull();
        await Assert.That(game.Turn).IsEqualTo(Color.White);
        await Assert.That(game.FullMoveCount).IsEqualTo(1);
        await Assert.That(game.HalfMoveCount).IsEqualTo(0);
    }

    [Test]
    public async Task Parse_WithInvalidFenTag_ThrowsInvalidPgnException()
    {
        var pgn = PgnFromMovetext("*", fen: "4k3/8/8/8/8/8/8/R3K3");
        void Act() => PgnParser.Parse(pgn);

        await Assert.That(Act)
            .ThrowsExactly<InvalidPgnException>()
            .WithInnerException<InvalidFenException>();
    }

    [Test]
    public async Task Parse_FenTagWithMovesInMovetext_BoardReflectsPositionAfterMoves()
    {
        var pgn = PgnFromMovetext("1. Ra4 *", fen: "4k3/8/8/8/8/8/8/R3K3 w Q - 0 1");

        var game = PgnParser.Parse(pgn);

        await Assert.That(game.Board["a4"].Piece).IsTypeOf<Rook>();
        await Assert.That(game.Board["a4"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(game.Board["a1"].Piece).IsNull();
        await Assert.That(game.Turn).IsEqualTo(Color.Black);
    }

    [Test]
    public async Task Parse_FenTagWithMovesInMovetext_NavigationReflectsCorrectPositionAtEachMove()
    {
        var pgn = PgnFromMovetext("1. Ra8+ Ke7 2. Ra7+ *", fen: "4k3/8/8/8/8/8/8/R3K3 w Q - 0 1");

        var game = PgnParser.Parse(pgn);

        await Assert.That(game.Board["a7"].Piece).IsTypeOf<Rook>();

        Game afterBlackMove = game.GoToPreviousMove();
        await Assert.That(afterBlackMove.Board["a8"].Piece).IsTypeOf<Rook>();
        await Assert.That(afterBlackMove.Board["e7"].Piece).IsTypeOf<King>();

        Game atStart = game.GoToMove(0);
        await Assert.That(atStart.Board["a1"].Piece).IsTypeOf<Rook>();
        await Assert.That(atStart.Board["a8"].Piece).IsNull();
        await Assert.That(atStart.Board["a7"].Piece).IsNull();
        await Assert.That(atStart.Board["e8"].Piece).IsTypeOf<King>();
    }

    [Test]
    public async Task Parse_FenTagWithBlackToMove_ParsesBlackFirstMove()
    {
        var pgn = PgnFromMovetext("1... a5 *", fen: "4k3/p7/8/8/8/8/8/4K3 b - - 0 1");

        var game = PgnParser.Parse(pgn);

        await Assert.That(game.Board["a5"].Piece).IsTypeOf<Pawn>();
        await Assert.That(game.Board["a5"].Piece!.Color).IsEqualTo(Color.Black);
        await Assert.That(game.Board["a7"].Piece).IsNull();
        await Assert.That(game.Turn).IsEqualTo(Color.White);

        var gameBeforeMove = game.GoToPreviousMove();
        await Assert.That(gameBeforeMove.Turn).IsEqualTo(Color.Black);
    }

    [Test]
    public async Task Parse_FenTagWithCastlingRights_CastlingRightsArePreserved()
    {
        var pgn = PgnFromMovetext("*", fen: "r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");

        var game = PgnParser.Parse(pgn);

        await Assert.That(game.CastlingAvailability.CanWhiteCastleKingside()).IsTrue();
        await Assert.That(game.CastlingAvailability.CanWhiteCastleQueenside()).IsTrue();
        await Assert.That(game.CastlingAvailability.CanBlackCastleKingside()).IsTrue();
        await Assert.That(game.CastlingAvailability.CanBlackCastleQueenside()).IsTrue();
    }

    [Test]
    public async Task Parse_FenTagWithCastlingRights_CastlingRightsAreUpdatedAfterCastling()
    {
        var pgn = PgnFromMovetext("1. O-O *", fen: "r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");

        var game = PgnParser.Parse(pgn);

        await Assert.That(game.CastlingAvailability.CanWhiteCastleKingside()).IsFalse();
        await Assert.That(game.CastlingAvailability.CanWhiteCastleQueenside()).IsFalse();
        await Assert.That(game.CastlingAvailability.CanBlackCastleKingside()).IsTrue();
        await Assert.That(game.CastlingAvailability.CanBlackCastleQueenside()).IsTrue();
    }

    [Test]
    public async Task Parse_FenTagWithEnPassantSquare_EnPassantIsAvailableAfterParsing()
    {
        var pgn = PgnFromMovetext("1. exd6 *", fen: "4k3/8/8/3pP3/8/8/8/4K3 w - d6 0 1");

        var game = PgnParser.Parse(pgn);

        await Assert.That(game.Board["d6"].Piece).IsTypeOf<Pawn>();
        await Assert.That(game.Board["d6"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(game.Board["d5"].Piece).IsNull();
        await Assert.That(game.Board["e5"].Piece).IsNull();
    }

    [Test]
    public async Task Parse_FenTagWithHalfMoveClock_HalfMoveClockIsPreservedAndUpdated()
    {
        var pgn = PgnFromMovetext("1. Ra4 *", fen: "4k3/8/8/8/8/8/8/R3K3 w Q - 5 1");

        var game = PgnParser.Parse(pgn);

        await Assert.That(game.HalfMoveCount).IsEqualTo(6);
    }

    [Test]
    public async Task Parse_FenTagWithFullMoveCount_FullMoveCountIsPreservedAndUpdated()
    {
        var pgn = PgnFromMovetext("10. Ra4 Kf8 *", fen: "4k3/8/8/8/8/8/8/R3K3 w Q - 0 10");

        var game = PgnParser.Parse(pgn);

        await Assert.That(game.FullMoveCount).IsEqualTo(11);
    }

    #endregion

    #region Basic moves

    [Test]
    public async Task Parse_EmptyMovetext_ReturnsInitialGame()
    {
        var pgn = PgnFromMovetext("*");
        var game = PgnParser.Parse(pgn);

        await Assert.That(game.CurrentMoveIndex).IsEqualTo(0);
        await Assert.That(game.Turn).IsEqualTo(Color.White);
    }

    [Test]
    public async Task Parse_BasicMoves_ParsesMoves()
    {
        var pgn = PgnFromMovetext("1. e4 e5 2. Nf3 Nc6 *");
        var game = PgnParser.Parse(pgn);

        await Assert.That(game.FullMoveCount).IsEqualTo(3);
        await Assert.That(game.HalfMoveCount).IsEqualTo(2);
        await Assert.That(game.CurrentMoveIndex).IsEqualTo(4);
        await Assert.That(game.Turn).IsEqualTo(Color.White);
        await Assert.That(game.Board["e2"].Piece).IsNull();
        await Assert.That(game.Board["e4"].Piece).IsTypeOf<Pawn>();
        await Assert.That(game.Board["e4"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(game.Board["e7"].Piece).IsNull();
        await Assert.That(game.Board["e5"].Piece).IsTypeOf<Pawn>();
        await Assert.That(game.Board["e5"].Piece!.Color).IsEqualTo(Color.Black);
        await Assert.That(game.Board["g1"].Piece).IsNull();
        await Assert.That(game.Board["f3"].Piece).IsTypeOf<Knight>();
        await Assert.That(game.Board["f3"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(game.Board["b8"].Piece).IsNull();
        await Assert.That(game.Board["c6"].Piece).IsTypeOf<Knight>();
        await Assert.That(game.Board["c6"].Piece!.Color).IsEqualTo(Color.Black);
    }

    [Test]
    public async Task Parse_BasicMoves_EqualsSerializeOutput()
    {
        var pgn = PgnFromMovetext("1. e4 e5 2. Nf3 Nc6 *");
        
        var game = PgnParser.Parse(pgn);
        string serializedFromParsed = PgnParser.Serialize(game);

        await Assert.That(serializedFromParsed).IsEqualTo(pgn);
    }

    [Test]
    public async Task Parse_PawnCapture_ParsesMoves()
    {
        var pgn = PgnFromMovetext("1. e4 d5 2. exd5 *");

        var game = PgnParser.Parse(pgn);

        await Assert.That(game.CurrentMoveIndex).IsEqualTo(3);
        await Assert.That(game.Turn).IsEqualTo(Color.Black);
        await Assert.That(game.Board["d5"].Piece).IsTypeOf<Pawn>();
        await Assert.That(game.Board["d5"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(game.Board["e4"].Piece).IsNull();
        await Assert.That(game.Board["d7"].Piece).IsNull();
    }

    [Test]
    public async Task Parse_EnPassant_ParsesMoves()
    {
        var pgn = PgnFromMovetext("1. e4 h6 2. e5 d5 3. exd6 *");

        var game = PgnParser.Parse(pgn);

        await Assert.That(game.CurrentMoveIndex).IsEqualTo(5);
        await Assert.That(game.Turn).IsEqualTo(Color.Black);
        await Assert.That(game.Board["d6"].Piece).IsTypeOf<Pawn>();
        await Assert.That(game.Board["d6"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(game.Board["d5"].Piece).IsNull();
        await Assert.That(game.Board["e5"].Piece).IsNull();
    }

    [Test]
    public async Task Parse_Checkmate_ParsesGameResultCorrectly()
    {
        var pgn = PgnFromMovetext("1. f3 e5 2. g4 Qh4# 0-1", "0-1");
        var game = PgnParser.Parse(pgn);

        await Assert.That(game.GameResult).IsEqualTo(GameResult.Checkmate);
    }

    [Test]
    public async Task Parse_Stalemate_ParsesGameResultCorrectly()
    {
        var pgn = PgnFromMovetext("1. 1/2-1/2", "1/2-1/2", fen: "8/8/8/8/8/5kq1/8/7K w - - 0 1");
        var game = PgnParser.Parse(pgn);

        await Assert.That(game.GameResult).IsEqualTo(GameResult.Stalemate);
    }

    [Test]
    public async Task Parse_DrawBySeventyFiveMoveRule_ParsesGameResultCorrectly()
    {
        var pgn = PgnFromMovetext("1/2-1/2", "1/2-1/2", fen: "8/p7/4k3/8/8/4K3/8/8 w - - 150 76");
        var game = PgnParser.Parse(pgn);

        await Assert.That(game.GameResult).IsEqualTo(GameResult.DrawBySeventyFiveMoveRule);
    }

    [Test]
    public async Task Parse_DrawByInsufficientMaterial_ParsesGameResultCorrectly()
    {
        var pgn = PgnFromMovetext("* 1/2-1/2", fen: "8/8/3k4/4b3/8/4B3/5K2/8 w - - 0 1");
        var game = PgnParser.Parse(pgn);

        await Assert.That(game.GameResult).IsEqualTo(GameResult.DrawByInsufficientMaterial);
    }

    [Test]
    public async Task Parse_DrawByFivefoldRepetition_ParsesGameResultCorrectly()
    {
        var pgn = PgnFromMovetext("1. Nf3 Nf6 2. Ng1 Ng8 3. Nf3 Nf6 4. Ng1 Ng8 5. Nf3 Nf6 6. Ng1 Ng8 7. Nf3 Nf6 8. Ng1 Ng8 1/2-1/2", "1/2-1/2");
        var game = PgnParser.Parse(pgn);

        await Assert.That(game.GameResult).IsEqualTo(GameResult.DrawByFivefoldRepetition);
    }

    [Test]
    public async Task Parse_DrawByAgreement_ParsesWithoutError()
    {
        // Since DrawByAgreement is not implemented this result should be InProgress
        var pgn = PgnFromMovetext("1. e4 e5 1/2-1/2", "1/2-1/2");
        var game = PgnParser.Parse(pgn);

        await Assert.That(game.GameResult).IsEqualTo(GameResult.InProgress);
        await Assert.That(game.Board["e4"].Piece).IsTypeOf<Pawn>();
        await Assert.That(game.Board["e5"].Piece).IsTypeOf<Pawn>();
    }

    #endregion

    #region Castling

    [Test]
    public async Task Parse_CanCastleKingside_ParsesCastlingAvailability()
    {
        var pgn = PgnFromMovetext("*", fen: "r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
        var game = PgnParser.Parse(pgn);
        
        var makeMoveResult = game.MakeMove("e1", "h1");
        await Assert.That(makeMoveResult.MoveResult).IsEqualTo(MoveResult.Success);
        
        game = makeMoveResult.Game;
        await Assert.That(game.Board["g1"].Piece).IsTypeOf<King>();
        await Assert.That(game.Board["g1"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(game.Board["f1"].Piece).IsTypeOf<Rook>();
        await Assert.That(game.Board["f1"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(game.Board["e1"].Piece).IsNull();
        await Assert.That(game.Board["h1"].Piece).IsNull();
    }

    [Test]
    public async Task Parse_CanNotCastleKingside_ParsesCastlingAvailability()
    {
        var pgn = PgnFromMovetext("*", fen: "r3k2r/8/8/8/8/8/8/R3K2R w Qkq - 0 1");
        var game = PgnParser.Parse(pgn);

        var makeMoveResult = game.MakeMove("e1", "h1");
        await Assert.That(makeMoveResult.MoveResult).IsEqualTo(MoveResult.IllegalMove);
    }

    [Test]
    public async Task Parse_HasCastledKingside_ParsesMoves()
    {
        var pgn = PgnFromMovetext("1. e4 e5 2. Nf3 Nc6 3. Bc4 Nf6 4. O-O *");
        var game = PgnParser.Parse(pgn);

        await Assert.That(game.Board["g1"].Piece).IsTypeOf<King>();
        await Assert.That(game.Board["g1"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(game.Board["f1"].Piece).IsTypeOf<Rook>();
        await Assert.That(game.Board["f1"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(game.Board["e1"].Piece).IsNull();
        await Assert.That(game.Board["h1"].Piece).IsNull();
    }

    [Test]
    public async Task Parse_CanCastleQueenside_ParsesCastlingAvailability()
    {
        var pgn = PgnFromMovetext("*", fen: "r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
        var game = PgnParser.Parse(pgn);

        var makeMoveResult = game.MakeMove("e1", "b1");
        await Assert.That(makeMoveResult.MoveResult).IsEqualTo(MoveResult.Success);

        game = makeMoveResult.Game;
        await Assert.That(game.Board["c1"].Piece).IsTypeOf<King>();
        await Assert.That(game.Board["c1"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(game.Board["d1"].Piece).IsTypeOf<Rook>();
        await Assert.That(game.Board["d1"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(game.Board["e1"].Piece).IsNull();
        await Assert.That(game.Board["a1"].Piece).IsNull();
    }

    [Test]
    public async Task Parse_CanNotCastleQueenside_ParsesCastlingAvailability()
    {
        var pgn = PgnFromMovetext("*", fen: "r3k2r/8/8/8/8/8/8/R3K2R w Kkq - 0 1");
        var game = PgnParser.Parse(pgn);

        var makeMoveResult = game.MakeMove("e1", "b1");
        await Assert.That(makeMoveResult.MoveResult).IsEqualTo(MoveResult.IllegalMove);
    }

    [Test]
    public async Task Parse_HasCastledQueenside_ParsesMoves()
    {
        var pgn = PgnFromMovetext("1. e4 d5 2. exd5 Qxd5 3. Nf3 Bg4 4. Be2 Nc6 5. d3 O-O-O *");

        var game = PgnParser.Parse(pgn);

        await Assert.That(game.Board["c8"].Piece).IsTypeOf<King>();
        await Assert.That(game.Board["c8"].Piece!.Color).IsEqualTo(Color.Black);
        await Assert.That(game.Board["d8"].Piece).IsTypeOf<Rook>();
        await Assert.That(game.Board["d8"].Piece!.Color).IsEqualTo(Color.Black);
        await Assert.That(game.Board["e8"].Piece).IsNull();
        await Assert.That(game.Board["a8"].Piece).IsNull();
    }

    #endregion

    #region Promotion

    [Test]
    public async Task Parse_PawnPromoted_ParsesMoves()
    {
        var pgn = PgnFromMovetext("a8=Q+ *", fen: "4k3/P7/8/8/8/8/8/4K3 w - - 0 1");
        var game = PgnParser.Parse(pgn);

        await Assert.That(game.Board["a8"].Piece).IsTypeOf<Queen>();
        await Assert.That(game.Board["a8"].Piece!.Color).IsEqualTo(Color.White);
        
        var gameBeforePromotion = game.GoToMove(0);
        await Assert.That(gameBeforePromotion.Board["a7"].Piece).IsTypeOf<Pawn>();
        await Assert.That(gameBeforePromotion.Board["a7"].Piece!.Color).IsEqualTo(Color.White);
    }

    [Test]
    public async Task Parse_PawnPromotedWithCapture_ParsesMoves()
    {
        var pgn = PgnFromMovetext("1. a7xb8=R+ *", fen: "1n2k3/P7/8/8/8/8/8/4K3 w - - 0 1");
        var game = PgnParser.Parse(pgn);

        await Assert.That(game.Board["b8"].Piece).IsTypeOf<Rook>();
        await Assert.That(game.Board["b8"].Piece!.Color).IsEqualTo(Color.White);

        var gameBeforePromotion = game.GoToMove(0);
        await Assert.That(gameBeforePromotion.Board["a7"].Piece).IsTypeOf<Pawn>();
        await Assert.That(gameBeforePromotion.Board["a7"].Piece!.Color).IsEqualTo(Color.White);
    }

    #endregion

    #region Disambiguation

    [Test]
    public async Task Parse_DisambiguationByFile_ParsesMoves()
    {
        var pgn = PgnFromMovetext("1. Raa4 *", fen: "4k3/8/8/8/3R4/8/8/R3K3 w Q - 0 1");
        var game = PgnParser.Parse(pgn);

        await Assert.That(game.Board["a4"].Piece).IsTypeOf<Rook>();
        await Assert.That(game.Board["a4"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(game.Board["a1"].Piece).IsNull();
        await Assert.That(game.Board["d4"].Piece).IsTypeOf<Rook>();

        var gameBeforeMove = game.GoToMove(0);
        await Assert.That(gameBeforeMove.Board["a1"].Piece).IsTypeOf<Rook>();
        await Assert.That(gameBeforeMove.Board["a1"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(gameBeforeMove.Board["a4"].Piece).IsNull();
        await Assert.That(gameBeforeMove.Board["d4"].Piece).IsTypeOf<Rook>();
    }

    [Test]
    public async Task Parse_DisambiguationByRank_ParsesMoves()
    {
        var pgn = PgnFromMovetext("1. R7a5 *", fen: "4k3/R7/8/8/8/8/8/R3K3 w Q - 0 1");
        var game = PgnParser.Parse(pgn);

        await Assert.That(game.Board["a5"].Piece).IsTypeOf<Rook>();
        await Assert.That(game.Board["a5"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(game.Board["a7"].Piece).IsNull();

        var gameBeforeMove = game.GoToMove(0);
        await Assert.That(gameBeforeMove.Board["a7"].Piece).IsTypeOf<Rook>();
        await Assert.That(gameBeforeMove.Board["a7"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(gameBeforeMove.Board["a5"].Piece).IsNull();
    }

    #endregion

    #region Variations

    [Test]
    public async Task Parse_WithVariation_EndsAtMainLineTip()
    {
        var pgn = PgnFromMovetext("1. e4 (1. d4) 1... e5 *");
        Game game = PgnParser.Parse(pgn);

        await Assert.That(game.CurrentMoveIndex).IsEqualTo(2);
    }

    [Test]
    public async Task Parse_SingleVariation_ParsesMainLineAndVariation()
    {
        var pgn = PgnFromMovetext("1. e4 (1. d4) *");
        Game game = PgnParser.Parse(pgn);

        await Assert.That(game.CurrentMoveIndex).IsEqualTo(1);
        await Assert.That(game.CurrentBranchLength).IsEqualTo(1);
        await Assert.That(game.Turn).IsEqualTo(Color.Black);
        await Assert.That(game.Board["e4"].Piece).IsTypeOf<Pawn>();
        await Assert.That(game.Board["e4"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(game.Board["e2"].Piece).IsNull();
        
        Game variation = game.GoToPreviousMove().GoToVariation(1);
        await Assert.That(variation.Board["d4"].Piece).IsTypeOf<Pawn>();
        await Assert.That(variation.Board["d4"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(variation.Board["d2"].Piece).IsNull();
        await Assert.That(variation.Board["e4"].Piece).IsNull();
    }

    [Test]
    public async Task Parse_MultiMoveVariation_ParsesFullVariation()
    {
        var pgn = PgnFromMovetext("1. e4 (1. d4 d5) *");
        Game game = PgnParser.Parse(pgn);

        await Assert.That(game.CurrentMoveIndex).IsEqualTo(1);
        await Assert.That(game.CurrentBranchLength).IsEqualTo(1);
        await Assert.That(game.Turn).IsEqualTo(Color.Black);
        await Assert.That(game.Board["e4"].Piece).IsTypeOf<Pawn>();
        await Assert.That(game.Board["e4"].Piece!.Color).IsEqualTo(Color.White);
        
        // Variation: d4 then d5
        Game variationFirstMove = game.GoToPreviousMove().GoToVariation(1);
        await Assert.That(variationFirstMove.Board["d4"].Piece).IsTypeOf<Pawn>();
        await Assert.That(variationFirstMove.Board["d4"].Piece!.Color).IsEqualTo(Color.White);
        Game variationSecondMove = variationFirstMove.GoToNextMove();
        await Assert.That(variationSecondMove.Board["d4"].Piece).IsTypeOf<Pawn>();
        await Assert.That(variationSecondMove.Board["d4"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(variationSecondMove.Board["d5"].Piece).IsTypeOf<Pawn>();
        await Assert.That(variationSecondMove.Board["d5"].Piece!.Color).IsEqualTo(Color.Black);
        await Assert.That(variationSecondMove.Turn).IsEqualTo(Color.White);
    }

    [Test]
    public async Task Parse_MultipleVariationsAtSamePoint_ParsesAllVariations()
    {
        var pgn = PgnFromMovetext("1. e4 (1. d4) (1. c4) *");
        Game result = PgnParser.Parse(pgn);

        await Assert.That(result.CurrentMoveIndex).IsEqualTo(1);
        await Assert.That(result.Turn).IsEqualTo(Color.Black);
        await Assert.That(result.Board["e4"].Piece).IsTypeOf<Pawn>();
        await Assert.That(result.Board["e4"].Piece!.Color).IsEqualTo(Color.White);
        
        Game atRoot = result.GoToPreviousMove();
        await Assert.That(atRoot.GoToVariation(0).Board["e4"].Piece).IsTypeOf<Pawn>();
        await Assert.That(atRoot.GoToVariation(1).Board["d4"].Piece).IsTypeOf<Pawn>();
        await Assert.That(atRoot.GoToVariation(1).Board["e4"].Piece).IsNull();
        await Assert.That(atRoot.GoToVariation(2).Board["c4"].Piece).IsTypeOf<Pawn>();
        await Assert.That(atRoot.GoToVariation(2).Board["e4"].Piece).IsNull();
    }

    [Test]
    public async Task Parse_NestedVariation_ParsesAllVariations()
    {
        var pgn = PgnFromMovetext("1. e4 (1. d4 d5 2. c4 (2. Nf3)) 1... e5 *");
        Game result = PgnParser.Parse(pgn);

        // 1. e4 e5 main line
        await Assert.That(result.CurrentMoveIndex).IsEqualTo(2);
        await Assert.That(result.Turn).IsEqualTo(Color.White);
        await Assert.That(result.Board["e4"].Piece).IsTypeOf<Pawn>();
        await Assert.That(result.Board["e4"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(result.Board["e5"].Piece).IsTypeOf<Pawn>();
        await Assert.That(result.Board["e5"].Piece!.Color).IsEqualTo(Color.Black);

        // 1. d4 d5 2. c4 variation
        Game d4Variation = result.GoToPreviousMove().GoToPreviousMove().GoToVariation(1);
        await Assert.That(d4Variation.Board["d4"].Piece).IsTypeOf<Pawn>();
        await Assert.That(d4Variation.Board["d4"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(d4Variation.Board["e4"].Piece).IsNull();
        Game afterD5 = d4Variation.GoToNextMove();
        await Assert.That(afterD5.Board["d5"].Piece).IsTypeOf<Pawn>();
        await Assert.That(afterD5.Board["d5"].Piece!.Color).IsEqualTo(Color.Black);
        await Assert.That(afterD5.Turn).IsEqualTo(Color.White);
        Game afterC4 = afterD5.GoToNextMove();
        await Assert.That(afterC4.Board["c4"].Piece).IsTypeOf<Pawn>();
        await Assert.That(afterC4.Board["c4"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(afterC4.Board["f3"].Piece).IsNull();

        // 2. Nf3 variation
        Game nf3Variation = afterD5.GoToVariation(1);
        await Assert.That(nf3Variation.Board["f3"].Piece).IsTypeOf<Knight>();
        await Assert.That(nf3Variation.Board["f3"].Piece!.Color).IsEqualTo(Color.White);
        await Assert.That(nf3Variation.Board["c4"].Piece).IsNull();
    }

    #endregion

    [Test]
    public async Task Parse_FullGameWithTagsVariationsAndComments_ParsesGame()
    {
        var pgn = """
[Event "Paris"]
[Site "Paris FRA"]
[Date "1858.??.??"]
[EventDate "?"]
[Round "?"]
[Result "1-0"]
[White "Paul Morphy"]
[Black "Duke Karl / Count Isouard"]
[ECO "C41"]
[WhiteElo "?"]
[BlackElo "?"]
[PlyCount "33"]

1. e4 {Notes by Stockfish 8 v270317 (minimum 60s/ply)} e5 2. Nf3
d6 3. d4 Bg4 {better is 3... Nf6 4. Nc3 exd4 5. Nxd4 Nc6 6. Bc4 Be7
7. O-O O-O = +0.23 (37 ply)} 4. dxe5 {+/= +0.95 (34 ply)} Bxf3
5. Qxf3 {5. gxf3 was played in Breyer vs R Binder, 1921 (1-0)}
dxe5 6. Bc4 {6. Qb3 was played in V Neverov vs M Chahrour, 2009
(1-0)} Nf6 7. Qb3 {7. Bg5 was played in M Spaeti vs J
Soltermann, 2005 (1-0)} Qe7 8. Nc3 {8. Qxb7 was played in A
Milovic vs D Mihajlova, 2008 (1-0)} c6 9. Bg5 b5 {? 9... Na6
10. Bxa6 bxa6 11. Rd1 Qb4 12. Ke2 a5 13. Rd3 Be7 +/- +1.54 (41
ply)} 10. Nxb5 {+- +2.72 (42 ply)} cxb5 {? 10... Qb4+ 11. Qxb4
Bxb4+ 12. c3 cxb5 13. Bxb5+ Nbd7 14. cxb4 +- +2.79 (39 ply)}
11. Bxb5+ {+- +7.42 (39 ply)} Nbd7 12. O-O-O Rd8 13. Rxd7 Rxd7
14. Rd1 Qe6 15. Bxd7+ Nxd7 16. Qb8+ Nxb8 17. Rd8# 1-0
""";

        var game = PgnParser.Parse(pgn);

        await Assert.That(game.FullMoveCount).IsEqualTo(17);
        await Assert.That(game.HalfMoveCount).IsEqualTo(1);
        await Assert.That(game.GameResult).IsEqualTo(GameResult.Checkmate);
        await Assert.That(game.CastlingAvailability.CanBlackCastleKingside()).IsTrue();
    }
}

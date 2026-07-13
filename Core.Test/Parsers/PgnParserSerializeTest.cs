using Core.ChessGame;
using Core.Parsers;
using Core.Pieces;
using Core.Shared;
using System;
using System.Threading.Tasks;

namespace Core.Test.Parsers;

public class PgnParserSerializeTest
{
    private static string Movetext(Game game)
    {
        string pgn = PgnParser.Serialize(game);
        // Movetext starts after the blank line that separates tags from movetext
        int blankLine = pgn.IndexOf("\r\n\r\n");
        if (blankLine < 0) blankLine = pgn.IndexOf("\n\n");
        return pgn[(blankLine + 2)..].Trim().Replace("\r\n", " ").Replace("\n", " ");
    }

    #region Tags

    [Test]
    public async Task Serialize_ContainsAllSevenRosterTags()
    {
        string pgn = PgnParser.Serialize(new Game());

        await Assert.That(pgn).Contains("[Event \"?\"]");
        await Assert.That(pgn).Contains("[Site \"?\"]");
        await Assert.That(pgn).Contains("[Date \"????.??.??\"]");
        await Assert.That(pgn).Contains("[Round \"?\"]");
        await Assert.That(pgn).Contains("[White \"?\"]");
        await Assert.That(pgn).Contains("[Black \"?\"]");
        await Assert.That(pgn).Contains("[Result \"*\"]");
    }

    [Test]
    public async Task Serialize_CheckmateByWhite_ResultTagIsOneZero()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;
        game = game.MakeMove("e7", "e5").Game;
        game = game.MakeMove("f1", "c4").Game;
        game = game.MakeMove("b8", "c6").Game;
        game = game.MakeMove("d1", "h5").Game;
        game = game.MakeMove("a7", "a6").Game;
        game = game.MakeMove("h5", "f7").Game;

        string pgn = PgnParser.Serialize(game);

        await Assert.That(pgn).Contains("[Result \"1-0\"]");
    }

    [Test]
    public async Task Serialize_CheckmateByBlack_ResultTagIsZeroOne()
    {
        var game = new Game();
        game = game.MakeMove("f2", "f3").Game;
        game = game.MakeMove("e7", "e5").Game;
        game = game.MakeMove("g2", "g4").Game;
        game = game.MakeMove("d8", "h4").Game;

        string pgn = PgnParser.Serialize(game);

        await Assert.That(pgn).Contains("[Result \"0-1\"]");
    }

    [Test]
    public async Task Serialize_GameInProgress_ResultTagIsStar()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;

        string pgn = PgnParser.Serialize(game);

        await Assert.That(pgn).Contains("[Result \"*\"]");
    }

    [Test]
    public async Task Serialize_DrawByAgreement_ResultTagIsHalf()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;
        game = game.ClaimDraw();

        string pgn = PgnParser.Serialize(game);

        await Assert.That(pgn).Contains("[Result \"1/2-1/2\"]");
    }

    #endregion

    #region Pawn moves

    [Test]
    public async Task Serialize_PawnMove_NoLetterPrefix()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;

        await Assert.That(Movetext(game)).Contains("1. e4");
        await Assert.That(Movetext(game)).DoesNotContain("Pe4");
    }

    [Test]
    public async Task Serialize_PawnCapture_IncludesOriginFileAndX()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;
        game = game.MakeMove("d7", "d5").Game;
        game = game.MakeMove("e4", "d5").Game;

        await Assert.That(Movetext(game)).Contains("exd5");
    }

    #endregion

    #region Piece moves

    [Test]
    public async Task Serialize_KnightMove_HasNPrefix()
    {
        var game = new Game();
        game = game.MakeMove("g1", "f3").Game;

        await Assert.That(Movetext(game)).Contains("1. Nf3");
    }

    [Test]
    public async Task Serialize_PieceCapture_IncludesX()
    {
        var game = FenParser.Parse("4k3/8/8/8/8/8/p7/R3K3 w Q - 0 1");
        game = game.MakeMove("a1", "a2").Game;

        await Assert.That(Movetext(game)).Contains("Rxa2");
    }

    #endregion

    #region Castling

    [Test]
    public async Task Serialize_KingsideCastling_IsOO()
    {
        var game = FenParser.Parse("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
        game = game.MakeMove("e1", "h1").Game;

        await Assert.That(Movetext(game)).Contains("O-O");
    }

    [Test]
    public async Task Serialize_QueensideCastling_IsOOO()
    {
        var game = FenParser.Parse("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
        game = game.MakeMove("e1", "b1").Game;

        await Assert.That(Movetext(game)).Contains("O-O-O");
    }

    [Test]
    public async Task Serialize_KingsideCastling_IsNotOOO()
    {
        var game = FenParser.Parse("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
        game = game.MakeMove("e1", "h1").Game;

        var movetext = Movetext(game);
        await Assert.That(movetext).Contains("O-O");
        await Assert.That(movetext).DoesNotContain("O-O-O");
    }

    #endregion

    #region Promotion

    [Test]
    public async Task Serialize_PawnPromotion_IncludesEqualsAndPiece()
    {
        var game = FenParser.Parse("4k3/P7/8/8/8/8/8/4K3 w - - 0 1");
        game = game.MakeMove("a7", "a8", 'N').Game;

        await Assert.That(Movetext(game)).Contains("a8=N");
    }

    [Test]
    public async Task Serialize_PawnPromotionCapture_IncludesCaptureAndPromotion()
    {
        var game = FenParser.Parse("1n2k3/P7/8/8/8/8/8/4K3 w - - 0 1");
        game = game.MakeMove("a7", "b8", 'B').Game;

        await Assert.That(Movetext(game)).Contains("axb8=B");
    }

    [Test]
    public async Task Serialize_PawnPromotionWithCheck_IncludesEqualsPieceAndPlus()
    {
        var game = FenParser.Parse("4k3/P7/8/8/8/8/8/4K3 w - - 0 1");
        game = game.MakeMove("a7", "a8", 'Q').Game;

        await Assert.That(Movetext(game)).Contains("a8=Q+");
    }

    [Test]
    [Arguments('Q', "a8=Q")]
    [Arguments('R', "a8=R")]
    [Arguments('B', "a8=B")]
    [Arguments('N', "a8=N")]
    public async Task Serialize_WhitePromotion_IncludesCorrectPieceLetter(char piece, string expected)
    {
        var game = FenParser.Parse("4k3/P7/8/8/8/8/8/4K3 w - - 0 1");
        game = game.MakeMove("a7", "a8", piece).Game;

        await Assert.That(Movetext(game)).Contains(expected);
    }

    [Test]
    [Arguments('q', "a1=Q")]
    [Arguments('r', "a1=R")]
    [Arguments('b', "a1=B")]
    [Arguments('n', "a1=N")]
    public async Task Serialize_BlackPromotion_IncludesCorrectPieceLetter(char piece, string expected)
    {
        var game = FenParser.Parse("4K3/8/8/8/8/8/p7/4k3 b - - 0 1");
        game = game.MakeMove("a2", "a1", piece).Game;

        await Assert.That(Movetext(game)).Contains(expected);
    }

    [Test]
    [Arguments('Q')]
    [Arguments('R')]
    [Arguments('B')]
    [Arguments('N')]
    public async Task Serialize_WhitePromotion_RoundTrip_ParsedPieceMatchesPromotedPiece(char piece)
    {
        var game = FenParser.Parse("4k3/P7/8/8/8/8/8/4K3 w - - 0 1");
        game = game.MakeMove("a7", "a8", piece).Game;

        string pgn = PgnParser.Serialize(game);
        Game parsed = PgnParser.Parse(pgn);

        Type expectedType = piece switch
        {
            'Q' => typeof(Queen),
            'R' => typeof(Rook),
            'B' => typeof(Bishop),
            'N' => typeof(Knight),
            _ => throw new InvalidOperationException()
        };
        await Assert.That(parsed.Board["a8"].Piece).IsOfType(expectedType);
        await Assert.That(parsed.Board["a8"].Piece!.Color).IsEqualTo(Color.White);
    }

    #endregion

    #region Check and checkmate

    [Test]
    public async Task Serialize_MoveThatGivesCheck_HasPlusSuffix()
    {
        var game = FenParser.Parse("4k3/8/8/8/8/8/8/R3K3 w Q - 0 1");
        game = game.MakeMove("a1", "a8").Game;

        await Assert.That(Movetext(game)).Contains("Ra8+");
    }

    [Test]
    public async Task Serialize_Checkmate_HasHashSuffix()
    {
        var game = new Game();
        game = game.MakeMove("f2", "f3").Game;
        game = game.MakeMove("e7", "e5").Game;
        game = game.MakeMove("g2", "g4").Game;
        game = game.MakeMove("d8", "h4").Game;

        await Assert.That(Movetext(game)).Contains("Qh4#");
    }

    #endregion

    #region Move numbers

    [Test]
    public async Task Serialize_FirstMove_StartsWithMoveNumber()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;

        await Assert.That(Movetext(game)).StartsWith("1.");
    }

    [Test]
    public async Task Serialize_TwoFullMoves_ContainsTwoMoveNumbers()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;
        game = game.MakeMove("e7", "e5").Game;
        game = game.MakeMove("g1", "f3").Game;
        game = game.MakeMove("b8", "c6").Game;

        string movetext = Movetext(game);
        await Assert.That(movetext).Contains("1. e4 e5");
        await Assert.That(movetext).Contains("2. Nf3 Nc6");
    }

    [Test]
    public async Task Serialize_BlackMoveOnly_DoesNotRepeatMoveNumber()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;
        game = game.MakeMove("e7", "e5").Game;
        game = game.MakeMove("g1", "f3").Game;
        game = game.MakeMove("g8", "f6").Game;

        string movetext = Movetext(game);
        await Assert.That(movetext).Contains("1. e4 e5");
    }

    #endregion

    #region Result token

    [Test]
    public async Task Serialize_EmptyGame_MovetextIsStar()
    {
        var game = new Game();

        await Assert.That(Movetext(game)).IsEqualTo("*");
    }

    [Test]
    public async Task Serialize_InProgress_MovetextEndsWithStar()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;

        await Assert.That(Movetext(game)).EndsWith("*");
    }

    [Test]
    public async Task Serialize_Checkmate_MovetextEndsWithResultToken()
    {
        var game = new Game();
        game = game.MakeMove("f2", "f3").Game;
        game = game.MakeMove("e7", "e5").Game;
        game = game.MakeMove("g2", "g4").Game;
        game = game.MakeMove("d8", "h4").Game;

        await Assert.That(Movetext(game)).EndsWith("0-1");
    }

    [Test]
    public async Task Serialize_DrawByAgreement_MovetextEndsWithHalf()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;
        game = game.ClaimDraw();

        await Assert.That(Movetext(game)).EndsWith("1/2-1/2");
    }

    #endregion

    #region Disambiguation

    [Test]
    public async Task Serialize_TwoRooksCanReachSameSquare_DisambiguatesWithFile()
    {
        var game = FenParser.Parse("4k3/8/8/8/3R4/8/8/R3K3 w Q - 0 1");
        game = game.MakeMove("a1", "a4").Game;

        await Assert.That(Movetext(game)).Contains("Raa4");
    }

    [Test]
    public async Task Serialize_TwoRooksOnSameFile_DisambiguatesWithRank()
    {
        var game = FenParser.Parse("4k3/R7/8/8/8/8/8/R3K3 w Q - 0 1");
        game = game.MakeMove("a7", "a3").Game;

        await Assert.That(Movetext(game)).Contains("R7a3");
    }

    [Test]
    public async Task Serialize_ThreeQueens_DisambiguatesWithFileAndRank()
    {
        var game = FenParser.Parse("4k3/8/8/8/3Q3Q/8/3Q4/4K3 w - - 0 1");
        game = game.MakeMove("d4", "f4").Game;

        await Assert.That(Movetext(game)).Contains("Qd4f4");
    }

    [Test]
    public async Task Serialize_TwoPiecesOfWhichOneIsPinned_DoesNotDisambiguate()
    {
        var game = FenParser.Parse("4k3/8/8/4r3/8/4N1N1/8/4K3 w - - 0 1");
        game = game.MakeMove("g3", "f5").Game;

        await Assert.That(Movetext(game)).Contains("Nf5");
        await Assert.That(Movetext(game)).DoesNotContain("Ngf5");
    }

    #endregion

    #region Variations

    [Test]
    public async Task Serialize_SingleVariation_WrapsInParentheses()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;

        Game withVariation = game.GoToMove(0).MakeMove("d2", "d4").Game;

        string movetext = Movetext(withVariation);
        await Assert.That(movetext).Contains("(");
        await Assert.That(movetext).Contains(")");
    }

    [Test]
    public async Task Serialize_VariationAfterWhiteMove_ContainsVariationMove()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;

        Game withVariation = game.GoToMove(0).MakeMove("d2", "d4").Game;

        string movetext = Movetext(withVariation);
        await Assert.That(movetext).Contains("1. e4 (1. d4)");
    }

    #endregion

    #region En passant

    [Test]
    public async Task Serialize_EnPassantCapture_IncludesOriginFileAndX()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;
        game = game.MakeMove("h7", "h6").Game;
        game = game.MakeMove("e4", "e5").Game;
        game = game.MakeMove("d7", "d5").Game;
        game = game.MakeMove("e5", "d6").Game;

        await Assert.That(Movetext(game)).Contains("exd6");
    }

    #endregion

    #region Black castling

    [Test]
    public async Task Serialize_BlackKingsideCastling_IsOO()
    {
        var game = FenParser.Parse("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
        game = game.MakeMove("h1", "h2").Game;
        game = game.MakeMove("e8", "h8").Game;

        await Assert.That(Movetext(game)).Contains("O-O");
    }

    #endregion

    #region Castling with check

    [Test]
    public async Task Serialize_CastlingWithCheck_HasPlusSuffix()
    {
        var game = FenParser.Parse("4k2r/8/8/8/8/8/8/5K2 b k - 0 1");
        game = game.MakeMove("e8", "h8").Game;

        await Assert.That(Movetext(game)).Contains("O-O+");
    }

    #endregion

    #region Non-default starting move number

    [Test]
    public async Task Serialize_GameStartingAtMoveFour_MovetextStartsWithFour()
    {
        var game = FenParser.Parse("rnbqkbnr/ppp1pppp/8/8/8/2N2N2/PPPP1PPP/R1BQKB1R b KQkq - 3 4");
        game = game.MakeMove("d8", "d2").Game;

        await Assert.That(Movetext(game)).StartsWith("4...");
    }

    #endregion

    #region Draw result token

    [Test]
    public async Task Serialize_Stalemate_MovetextEndsWithDrawResult()
    {
        var game = FenParser.Parse("8/8/8/8/8/4q2k/8/7K b - - 0 1");
        game = game.MakeMove("e3", "g3").Game;

        await Assert.That(Movetext(game)).EndsWith("1/2-1/2");
    }

    #endregion

    #region Variation edge cases

    [Test]
    public async Task Serialize_VariationAfterBlackMove_WrapsInParentheses()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;
        game = game.MakeMove("e7", "e5").Game;

        Game withVariation = game.GoToMove(1).MakeMove("c7", "c5").Game;

        string movetext = Movetext(withVariation);
        await Assert.That(movetext).Contains("(1... c5)");
    }

    [Test]
    public async Task Serialize_VariationAfterBlackMove_RepeatsEllipsisMoveNumber()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;
        game = game.MakeMove("e7", "e5").Game;

        Game withVariation = game.GoToMove(1).MakeMove("c7", "c5").Game;

        string movetext = Movetext(withVariation);
        await Assert.That(movetext).Contains("1. e4 e5");
        await Assert.That(movetext).Contains("(1... c5)");
    }

    [Test]
    public async Task Serialize_MultiMoveVariation_ContainsAllVariationMoves()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;

        Game withVariation = game.GoToMove(0).MakeMove("d2", "d4").Game;
        withVariation = withVariation.MakeMove("d7", "d5").Game;

        await Assert.That(Movetext(withVariation)).Contains("(1. d4 d5)");
    }

    [Test]
    public async Task Serialize_MultipleVariationsAtSamePoint_ContainsBothVariations()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;

        Game withFirstVariation = game.GoToMove(0).MakeMove("d2", "d4").Game;
        Game withSecondVariation = withFirstVariation.GoToMove(0).MakeMove("c2", "c4").Game;

        string movetext = Movetext(withSecondVariation);
        await Assert.That(movetext).Contains("1. e4 (1. d4) (1. c4)");
    }

    [Test]
    public async Task Serialize_NestedVariation_WrapsInNestedParentheses()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;
        game = game.MakeMove("e7", "e5").Game;

        Game withVariation = game.GoToMove(0).MakeMove("d2", "d4").Game;
        Game withNestedVariation = withVariation.GoToMove(1).MakeMove("d7", "d5").Game;

        await Assert.That(Movetext(withNestedVariation)).Contains("1. e4 (1. d4 d5) 1... e5");
    }

    [Test]
    public async Task Serialize_VariationWithInternalMoveNumber_HasMoveNumberInsideVariation()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;

        Game withVariation = game.GoToMove(0).MakeMove("d2", "d4").Game;
        withVariation = withVariation.MakeMove("d7", "d5").Game;
        withVariation = withVariation.MakeMove("c2", "c4").Game;

        await Assert.That(Movetext(withVariation)).Contains("(1. d4 d5 2. c4)");
    }

    [Test]
    public async Task Serialize_VariationAfterBlackMove_MainLineContinues_HasNextWhiteMoveNumber()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;
        game = game.MakeMove("e7", "e5").Game;
        game = game.MakeMove("g1", "f3").Game;

        Game withVariation = game.GoToMove(1).MakeMove("c7", "c5").Game;

        await Assert.That(Movetext(withVariation)).Contains("e5 (1... c5) 2. Nf3");
    }

    [Test]
    public async Task Serialize_MultipleVariationsWithMainLineContinuation_HasMoveNumberAfterVariations()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;
        game = game.MakeMove("e7", "e5").Game;

        Game withFirstVariation = game.GoToMove(0).MakeMove("d2", "d4").Game;
        Game withSecondVariation = withFirstVariation.GoToMove(0).MakeMove("c2", "c4").Game;

        await Assert.That(Movetext(withSecondVariation)).Contains("1. e4 (1. d4) (1. c4) 1... e5");
    }

    [Test]
    public async Task Serialize_VariationWithSubVariation_HasNestedParentheses()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;
        game = game.MakeMove("e7", "e5").Game;

        Game withVariation = game.GoToMove(0).MakeMove("d2", "d4").Game;
        withVariation = withVariation.MakeMove("d7", "d5").Game;
        withVariation = withVariation.MakeMove("c2", "c4").Game;

        Game withSubVariation = withVariation.GoToPreviousMove().MakeMove("g1", "f3").Game;

        await Assert.That(Movetext(withSubVariation)).Contains("(1. d4 d5 2. c4 (2. Nf3))");
    }

    #endregion

    #region FEN tag export

    [Test]
    public async Task Serialize_DefaultStartPosition_DoesNotContainFenTag()
    {
        string pgn = PgnParser.Serialize(new Game());

        await Assert.That(pgn).DoesNotContain("[FEN");
    }

    [Test]
    public async Task Serialize_GameFromCustomFen_ContainsFenTag()
    {
        var fen = "r1bqkb1r/pppp1ppp/2n2n2/4p3/2B1P3/5N2/PPPP1PPP/RNBQK2R w KQkq - 4 4";
        Game game = FenParser.Parse(fen);

        string pgn = PgnParser.Serialize(game);

        await Assert.That(pgn).Contains($"[FEN \"{fen}\"]");
    }

    [Test]
    public async Task Serialize_GameFromCustomFenWithMovesMade_ContainsOriginalFenTag()
    {
        var fen = "r1bqkb1r/pppp1ppp/2n2n2/4p3/2B1P3/5N2/PPPP1PPP/RNBQK2R w KQkq - 4 4";
        Game game = FenParser.Parse(fen);
        game = game.MakeMove("e1", "g1").Game;

        string pgn = PgnParser.Serialize(game);

        await Assert.That(pgn).Contains($"[FEN \"{fen}\"]");
    }

    #endregion
}

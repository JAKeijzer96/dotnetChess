using Core.ChessGame;
using Core.Shared;
using System;
using System.Threading.Tasks;

namespace Core.Test.ChessGame;

public class BranchingTest
{
    private static Game ThreeMoveGame_e4_e5_Nf3()
    {
        var game = new Game();
        game = game.MakeMove("e2", "e4").Game;
        game = game.MakeMove("e7", "e5").Game;
        game = game.MakeMove("g1", "f3").Game;
        return game;
    }

    #region Move branch creation

    [Test]
    public async Task MakeMove_AfterNavigatingBack_ReturnsSuccess()
    {
        var sut = ThreeMoveGame_e4_e5_Nf3();
        var atMove1 = sut.GoToMove(1);

        var result = atMove1.MakeMove("d7", "d5");

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.Success);
    }

    [Test]
    public async Task MakeMove_AfterNavigatingBack_CurrentMoveIndexReflectsNewBranch()
    {
        var sut = ThreeMoveGame_e4_e5_Nf3();
        var atMove1 = sut.GoToMove(1);

        var variation = atMove1.MakeMove("d7", "d5").Game;

        await Assert.That(variation.CurrentMoveIndex).IsEqualTo(2);
    }

    [Test]
    public async Task MakeMove_AfterNavigatingBack_BoardStateReflectsVariationMove()
    {
        var sut = ThreeMoveGame_e4_e5_Nf3();
        var atMove1 = sut.GoToMove(1);

        var variation = atMove1.MakeMove("d7", "d5").Game;

        await Assert.That(variation.Board["d5"].Piece).IsNotNull();
        await Assert.That(variation.Board["d7"].Piece).IsNull();
    }

    [Test]
    public async Task MakeMove_AfterNavigatingBack_DoesNotMutateOriginalGame()
    {
        var sut = ThreeMoveGame_e4_e5_Nf3();
        var atMove1 = sut.GoToMove(1);
        atMove1.MakeMove("d7", "d5");

        await Assert.That(sut.CurrentMoveIndex).IsEqualTo(3);
        await Assert.That(sut.CurrentBranchLength).IsEqualTo(3);
    }

    #endregion

    #region Existing move node navigation

    [Test]
    public async Task MakeMove_ReplayingExistingMove_ReturnsSuccess()
    {
        var sut = new Game();
        sut = sut.MakeMove("e2", "e4").Game;
        sut = sut.MakeMove("e7", "e5").Game;
        var atStart = sut.GoToMove(0);

        var result = atStart.MakeMove("e2", "e4");

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.Success);
    }

    [Test]
    public async Task MakeMove_ReplayingExistingMove_DoesNotDuplicateHistory()
    {
        var sut = new Game();
        sut = sut.MakeMove("e2", "e4").Game;
        sut = sut.MakeMove("e7", "e5").Game;
        var atStart = sut.GoToMove(0);

        var result = atStart.MakeMove("e2", "e4").Game;

        await Assert.That(result.CurrentBranchLength).IsEqualTo(2);
    }

    [Test]
    public async Task MakeMove_ReplayingExistingMove_PreservesExistingContinuations()
    {
        var sut = new Game();
        sut = sut.MakeMove("e2", "e4").Game;
        sut = sut.MakeMove("e7", "e5").Game;
        var atStart = sut.GoToMove(0);

        var afterReplayedE4 = atStart.MakeMove("e2", "e4").Game;
        var afterNextMove = afterReplayedE4.GoToNextMove();

        await Assert.That(afterNextMove.Board["e5"].Piece).IsNotNull();
    }

    #endregion

    #region GoToVariation

    [Test]
    public async Task GoToVariation_ZeroIndex_NavigatesToMainLineContinuation()
    {
        var sut = ThreeMoveGame_e4_e5_Nf3();
        var atMove1 = sut.GoToMove(1);
        var variation = atMove1.MakeMove("d7", "d5").Game;
        var backToMove1 = variation.GoToPreviousMove();

        var result = backToMove1.GoToVariation(0);

        await Assert.That(result.CurrentMoveIndex).IsEqualTo(2);
        await Assert.That(result.Board["e5"].Piece).IsNotNull();
    }

    [Test]
    public async Task GoToVariation_OneIndex_NavigatesToSecondContinuation()
    {
        var sut = ThreeMoveGame_e4_e5_Nf3();
        var atMove1 = sut.GoToMove(1);
        var variation = atMove1.MakeMove("d7", "d5").Game;
        var backToMove1 = variation.GoToPreviousMove();

        var result = backToMove1.GoToVariation(1);

        await Assert.That(result.CurrentMoveIndex).IsEqualTo(2);
        await Assert.That(result.Board["d5"].Piece).IsNotNull();
    }

    [Test]
    public async Task GoToVariation_NegativeIndex_ThrowsArgumentOutOfRangeException()
    {
        var sut = ThreeMoveGame_e4_e5_Nf3();
        var atMove1 = sut.GoToMove(1);

        Game action() => atMove1.GoToVariation(-1);

        await Assert.That(action).ThrowsExactly<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task GoToVariation_IndexBeyondContinuations_ThrowsArgumentOutOfRangeException()
    {
        var sut = ThreeMoveGame_e4_e5_Nf3();
        var atMove1 = sut.GoToMove(1);

        Game action() => atMove1.GoToVariation(1);

        await Assert.That(action).ThrowsExactly<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task GoToVariation_DoesNotMutateOriginalGame()
    {
        var sut = ThreeMoveGame_e4_e5_Nf3();
        var atMove1 = sut.GoToMove(1);
        var variation = atMove1.MakeMove("d7", "d5").Game;
        var backToMove1 = variation.GoToPreviousMove();
        backToMove1.GoToVariation(1);

        await Assert.That(backToMove1.CurrentMoveIndex).IsEqualTo(1);
    }

    #endregion

    #region CurrentBranchLength with branches

    [Test]
    public async Task CurrentBranchLength_OnVariationBranch_ShowsVariationPathOnly()
    {
        var sut = ThreeMoveGame_e4_e5_Nf3();
        var atMove1 = sut.GoToMove(1);
        var variation = atMove1.MakeMove("d7", "d5").Game;

        await Assert.That(variation.CurrentBranchLength).IsEqualTo(2);
    }

    #endregion

    #region GoToNextMove with multiple continuations

    [Test]
    public async Task GoToNextMove_WithMultipleContinuations_FollowsMainLine()
    {
        var sut = ThreeMoveGame_e4_e5_Nf3();
        var atMove1 = sut.GoToMove(1);
        var variation = atMove1.MakeMove("d7", "d5").Game;
        var backToMove1 = variation.GoToPreviousMove();

        var result = backToMove1.GoToNextMove();

        await Assert.That(result.Board["e5"].Piece).IsNotNull();
        await Assert.That(result.Board["d5"].Piece).IsNull();
    }

    #endregion

    #region GoToMove with branches

    [Test]
    public async Task GoToMove_FromVariation_IndexBeyondVariationLength_ThrowsArgumentOutOfRangeException()
    {
        var sut = ThreeMoveGame_e4_e5_Nf3();
        var atMove1 = sut.GoToMove(1);
        var variation = atMove1.MakeMove("d7", "d5").Game;

        Game action() => variation.GoToMove(3);

        await Assert.That(action).ThrowsExactly<ArgumentOutOfRangeException>();
    }

    #endregion

    #region Move repetition in branches

    [Test]
    public async Task MakeMove_SamePositionAsHistoricalMove_CreatesNewNodeAtCurrentPosition()
    {
        var sut = new Game();
        sut = sut.MakeMove("g1", "f3").Game;
        sut = sut.MakeMove("b8", "c6").Game;
        sut = sut.MakeMove("f3", "g1").Game;
        sut = sut.MakeMove("c6", "b8").Game;
        var result = sut.MakeMove("g1", "f3");

        await Assert.That(result.MoveResult).IsEqualTo(MoveResult.Success);
        await Assert.That(result.Game.CurrentMoveIndex).IsEqualTo(5);
        await Assert.That(result.Game.CurrentBranchLength).IsEqualTo(5);
    }

    [Test]
    public async Task MakeMove_SamePositionAsHistoricalMove_NavigatingBackFindsOriginalNode()
    {
        var sut = new Game();
        sut = sut.MakeMove("g1", "f3").Game;
        sut = sut.MakeMove("b8", "c6").Game;
        sut = sut.MakeMove("f3", "g1").Game;
        sut = sut.MakeMove("c6", "b8").Game;
        sut = sut.MakeMove("g1", "f3").Game;

        var atStart = sut.GoToMove(0);
        var result = atStart.MakeMove("g1", "f3").Game;

        await Assert.That(result.CurrentMoveIndex).IsEqualTo(1);
        await Assert.That(result.CurrentBranchLength).IsEqualTo(5);
    }

    #endregion
}

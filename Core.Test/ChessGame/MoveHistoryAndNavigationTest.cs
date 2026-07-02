using System;
using System.Threading.Tasks;
using Core.ChessGame;
using Core.Shared;

namespace Core.Test.ChessGame;

public class MoveHistoryAndNavigationTest
{
    #region MoveHistory

    [Test]
    public async Task MoveHistory_AtStartOfGame_IsEmpty()
    {
        var sut = new Game();

        await Assert.That(sut.MoveHistory).IsEmpty();
    }

    [Test]
    public async Task MoveHistory_AfterSuccessfulMove_ContainsOneEntry()
    {
        var sut = new Game();

        var game = sut.MakeMove("e2", "e4").Game;

        await Assert.That(game.MoveHistory).Count().IsEqualTo(1);
    }

    [Test]
    public async Task MoveHistory_AfterFailedMove_IsUnchanged()
    {
        var sut = new Game();

        var result = sut.MakeMove("e2", "b4");

        await Assert.That(result.Result).IsEqualTo(MoveResult.IllegalMove);
        await Assert.That(result.Game.MoveHistory).IsEmpty();
    }

    [Test]
    public async Task MoveHistory_AfterThreeMoves_ContainsThreeEntries()
    {
        var sut = new Game();
        sut = sut.MakeMove("e2", "e4").Game;
        sut = sut.MakeMove("e7", "e5").Game;
        sut = sut.MakeMove("g1", "f3").Game;

        await Assert.That(sut.MoveHistory).Count().IsEqualTo(3);
    }

    [Test]
    public async Task MoveHistory_RecordsCorrectFromAndToSquares()
    {
        var sut = new Game();
        var game = sut.MakeMove("e2", "e4").Game;

        var move = game.MoveHistory[0];

        await Assert.That(move.From.ToString()).IsEqualTo("e2");
        await Assert.That(move.To.ToString()).IsEqualTo("e4");
    }

    [Test]
    public async Task MoveHistory_FailedMoveReturnsOriginalGame()
    {
        var sut = new Game();
        var result = sut.MakeMove("e2", "d5");

        await Assert.That(result.Game).IsSameReferenceAs(sut);
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
    public async Task GoToMove_PreservesMoveHistory()
    {
        var sut = new Game();
        sut = sut.MakeMove("e2", "e4").Game;
        sut = sut.MakeMove("e7", "e5").Game;
        sut = sut.MakeMove("g1", "f3").Game;

        var result = sut.GoToMove(1);

        await Assert.That(result.MoveHistory).Count().IsEqualTo(3);
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

    #endregion
}

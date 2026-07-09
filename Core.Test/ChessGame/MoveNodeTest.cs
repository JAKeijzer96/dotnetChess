using Core.ChessBoard;
using Core.ChessGame;
using Core.Shared;
using System;
using System.Threading.Tasks;

namespace Core.Test.ChessGame;

public class MoveNodeTest
{
    private static Move AnyMove()
    {
        var random = new Random();
        var from = new Square((File)random.Next(8), (Rank)random.Next(8));
        var to = new Square((File)random.Next(8), (Rank)random.Next(8));
        return new(from, to, "PositionAfterMove", false, false);
    }

    #region Create

    [Test]
    public async Task Create_HasCorrectMove()
    {
        Move move = AnyMove();

        MoveNode sut = MoveNode.Create(move);

        await Assert.That(sut.Move).IsEqualTo(move);
    }

    [Test]
    public async Task Create_HasEmptyContinuations()
    {
        MoveNode sut = MoveNode.Create(AnyMove());

        await Assert.That(sut.Continuations).IsEmpty();
    }

    #endregion

    #region WithContinuation

    [Test]
    public async Task WithContinuation_AddsChildToContinuations()
    {
        MoveNode sut = MoveNode.Create(AnyMove());
        MoveNode child = MoveNode.Create(AnyMove());

        MoveNode result = sut.WithContinuation(child);

        await Assert.That(result.Continuations).Contains(child);
    }

    [Test]
    public async Task WithContinuation_DoesNotMutateOriginalNode()
    {
        MoveNode sut = MoveNode.Create(AnyMove());

        sut.WithContinuation(MoveNode.Create(AnyMove()));

        await Assert.That(sut.Continuations).IsEmpty();
    }

    [Test]
    public async Task WithContinuation_PreservesExistingContinuations()
    {
        MoveNode first = MoveNode.Create(AnyMove());
        MoveNode second = MoveNode.Create(AnyMove());
        MoveNode sut = MoveNode.Create(AnyMove()).WithContinuation(first);

        MoveNode result = sut.WithContinuation(second);

        await Assert.That(result.Continuations).Contains(first);
        await Assert.That(result.Continuations).Contains(second);
    }

    #endregion

    #region ReplaceContinuation

    [Test]
    public async Task ReplaceContinuation_ReplacesNodeAtIndex()
    {
        MoveNode original = MoveNode.Create(AnyMove());
        MoveNode replacement = MoveNode.Create(AnyMove());
        MoveNode sut = MoveNode.Create(AnyMove()).WithContinuation(original);

        MoveNode result = sut.ReplaceContinuation(0, replacement);

        await Assert.That(result.Continuations[0]).IsEqualTo(replacement);
    }

    [Test]
    public async Task ReplaceContinuation_DoesNotMutateOriginalNode()
    {
        MoveNode original = MoveNode.Create(AnyMove());
        MoveNode sut = MoveNode.Create(AnyMove()).WithContinuation(original);

        sut.ReplaceContinuation(0, MoveNode.Create(AnyMove()));

        await Assert.That(sut.Continuations[0]).IsEqualTo(original);
    }

    [Test]
    public async Task ReplaceContinuation_PreservesOtherContinuations()
    {
        MoveNode first = MoveNode.Create(AnyMove());
        MoveNode second = MoveNode.Create(AnyMove());
        MoveNode replacement = MoveNode.Create(AnyMove());
        MoveNode sut = MoveNode.Create(AnyMove())
            .WithContinuation(first)
            .WithContinuation(second);

        MoveNode result = sut.ReplaceContinuation(0, replacement);

        await Assert.That(result.Continuations[1]).IsEqualTo(second);
    }

    #endregion

}

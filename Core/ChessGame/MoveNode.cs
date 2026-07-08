using System.Collections.Immutable;
using Core.Shared;

namespace Core.ChessGame;

public record MoveNode(Move Move, ImmutableList<MoveNode> Continuations)
{
    public static MoveNode Create(Move move) => new(move, []);

    public MoveNode WithContinuation(MoveNode child)
        => this with { Continuations = Continuations.Add(child) };

    public MoveNode ReplaceContinuation(int index, MoveNode updated)
        => this with { Continuations = Continuations.SetItem(index, updated) };
}
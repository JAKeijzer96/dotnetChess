using System.Threading.Tasks;
using Core.ChessBoard;
using Core.MoveValidators;

namespace Core.Test.MoveValidators;

public class KnightMoveValidatorTest
{
    [Test]
    [Arguments("d5", "c3")] // 1 left 2 down
    [Arguments("d5", "b4")] // 2 left 1 down
    [Arguments("d5", "b6")] // 2 left 1 up
    [Arguments("d5", "c7")] // 1 left 2 up
    [Arguments("d5", "e7")] // 1 right 2 up
    [Arguments("d5", "f6")] // 2 right 1 up
    [Arguments("d5", "f4")] // 2 right 1 down
    [Arguments("d5", "e3")] // 1 right 2 down
    public async Task IsValidMove_ForAll8PossibleKnightMoves_ReturnsTrue(string from, string to)
    {
        var board = new Board("8/8/2q1p3/3N4/2B1P3/8/8/3k1K2");
        var validator = new KnightMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments("d5", "c6")]
    [Arguments("d5", "b3")]
    [Arguments("d5", "d3")]
    [Arguments("d5", "g8")]
    public async Task IsValidMove_ForInvalidKnightMove_ReturnsFalse(string from, string to)
    {
        var board = new Board("8/8/8/3N4/8/8/8/3k1K2");
        var validator = new KnightMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsFalse();
    }
}
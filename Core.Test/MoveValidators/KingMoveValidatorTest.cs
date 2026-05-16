using System.Threading.Tasks;
using Core.ChessBoard;
using Core.MoveValidators;

namespace Core.Test.MoveValidators;

public class KingMoveValidatorTest
{
    [Test]
    [Arguments("f6", "e6")] // Left
    [Arguments("f6", "g6")] // Right
    [Arguments("f6", "f7")] // Up
    [Arguments("f6", "f5")] // Down
    [Arguments("f6", "e5")] // Down left
    [Arguments("f6", "e7")] // Up left
    [Arguments("f6", "g7")] // Up right
    [Arguments("f6", "g5")] // Down right
    public async Task IsValidMove_ForOneSquareMove_ReturnsTrue(string from, string to)
    {
        var board = new Board("8/8/PK3k2/8/8/8/8/8");
        var validator = new KingMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments("f6", "d4")]
    [Arguments("f6", "d7")]
    [Arguments("f6", "f8")]
    [Arguments("f6", "h5")]
    [Arguments("f6", "h4")]
    public async Task IsValidMove_ForMultipleSquaresAtOnce_ReturnsFalse(string from, string to)
    {
        var board = new Board("8/8/PK3k2/8/8/8/8/8");
        var validator = new KingMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsFalse();
    }
}
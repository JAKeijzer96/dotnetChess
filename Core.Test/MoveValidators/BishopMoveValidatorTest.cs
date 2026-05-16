using System.Threading.Tasks;
using Core.ChessBoard;
using Core.MoveValidators;

namespace Core.Test.MoveValidators;

public class BishopMoveValidatorTest
{
    [Test]
    [Arguments("e4", "b1")] // Down left
    [Arguments("e4", "a8")] // Up left
    [Arguments("e4", "h7")] // Down right
    [Arguments("e4", "h1")] // Up right
    public async Task IsValidMove_ForDiagonalMove_ReturnsTrue(string from, string to)
    {
        var board = new Board("8/8/8/8/4B3/8/4P3/k3K3");
        var validator = new BishopMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments("e4", "e8")] // Up
    [Arguments("e4", "e1")] // Down
    [Arguments("e4", "a4")] // Left
    [Arguments("e4", "h4")] // Right
    public async Task IsValidMove_ForMoveThatIsNotDiagonal_ReturnsFalse(string from, string to)
    {
        var board = new Board("8/8/8/8/4B3/8/4P3/k3K3");
        var validator = new BishopMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments("d6", "f8")] // Blocked by opposite color
    [Arguments("d6", "h2")] // Blocked by same color
    public async Task IsValidMove_WhenMoveIsBlocked_ReturnsFalse(string from, string to)
    {
        var board = new Board("1N6/4P3/3b4/8/8/r5k1/8/5K2");
        var validator = new BishopMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsFalse();
    }
}
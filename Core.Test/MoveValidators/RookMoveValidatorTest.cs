using System.Threading.Tasks;
using Core.ChessBoard;
using Core.MoveValidators;

namespace Core.Test.MoveValidators;

public class RookMoveValidatorTest
{
    [Test]
    [Arguments("c5", "a5")] // Left
    [Arguments("c5", "f5")] // Right
    [Arguments("c5", "c6")] // Up
    [Arguments("c5", "c3")] // Down
    public async Task IsValidMove_ForOrthogonalMove_ReturnsTrue(string from, string to)
    {
        var board = new Board("8/2b5/8/2R5/8/8/k1K5/8");
        var validator = new RookMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments("c5", "a7")]
    [Arguments("c5", "b7")]
    [Arguments("c5", "g1")]
    [Arguments("c5", "a2")]
    public async Task IsValidMove_ForMoveThatIsNotHorizontalOrVertical_ReturnsFalse(string from, string to)
    {
        var board = new Board("8/2b5/8/2R5/8/8/k1K5/8");
        var validator = new RookMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments("c5", "c8")] // Blocked by opposite color
    [Arguments("c5", "c1")] // Blocked by same color
    public async Task IsValidMove_WhenMoveIsBlocked_ReturnsFalse(string from, string to)
    {
        var board = new Board("8/2b5/8/2R5/8/8/k1K5/8");
        var validator = new RookMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsFalse();
    }
}
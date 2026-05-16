using System.Threading.Tasks;
using Core.ChessBoard;
using Core.MoveValidators;

namespace Core.Test.MoveValidators;

public class PawnMoveValidatorTest
{
    [Test]
    [Arguments("d2", "d3")]
    [Arguments("d7", "d6")]
    public async Task IsValidMove_OneForwardOnFirstMove_IsTrue(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments("d2", "d4")]
    [Arguments("d7", "d5")]
    public async Task IsValidMove_TwoForwardOnFirstMove_IsTrue(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments("f2", "f3")]
    [Arguments("f7", "f6")]
    public async Task IsValidMove_OneForwardWithBlockingPieceInFront_IsFalse(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments("e2", "e4")]
    [Arguments("e7", "e5")]
    public async Task IsValidMove_TwoForwardWithBlockingPieceOnDestinationSquare_IsFalse(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments("f2", "f4")]
    [Arguments("f7", "f5")]
    public async Task IsValidMove_TwoForwardWithBlockingPieceBetweenFromAndTo_IsFalse(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments("g3", "g5")]
    [Arguments("g6", "g4")]
    public async Task IsValidMove_TwoForwardWithoutItBeingFirstMove_IsFalse(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments("d2", "d1")]
    [Arguments("d7", "d8")]
    public async Task IsValidMove_OneBackward_IsFalse(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments("d2", "c3")]
    [Arguments("e2", "f3")]
    [Arguments("d7", "c6")]
    [Arguments("e7", "f6")]
    public async Task IsValidMove_CapturingOtherColorPieceOneSquareDiagonally_IsTrue(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments("f2", "g3")]
    [Arguments("f7", "g6")]
    public async Task IsValidMove_CapturingSameColorPieceOneSquareDiagonally_IsFalse(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments("f2", "c5")]
    [Arguments("f7", "c4")]
    public async Task IsValidMove_CapturingOtherColorPieceMultipleSquaresDiagonally_IsFalse(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments("e2", "d3")]
    [Arguments("e7", "d6")]
    public async Task IsValidMove_MovingDiagonallyToEmptySquare_IsFalse(string from, string to)
    {
        var board = new Board("8/3ppp2/2Q2Qp1/2q1Q3/2Q1q3/2q2qP1/3PPP2/5K1k");
        var validator = new PawnMoveValidator();
        var fromSquare = board[from];
        var toSquare = board[to];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsFalse();
    }
}
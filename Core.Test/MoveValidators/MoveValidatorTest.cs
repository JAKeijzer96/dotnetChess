using System.Threading.Tasks;
using Core.ChessBoard;
using Core.MoveValidators;

namespace Core.Test.MoveValidators;

public class MoveValidatorTest
{
    [Test]
    public async Task IsValidMove_WhenPieceIsNull_ReturnsFalse()
    {
        var board = new Board("8/8/8/8/4B3/8/4P3/k3K3");
        MoveValidator validator = new KingMoveValidator();
        var fromSquare = board["d5"];
        var toSquare = board["c6"];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task IsValidMove_ToSameSquare_ReturnsFalse()
    {
        var board = new Board("8/8/8/8/4B3/8/4P3/k3K3");
        MoveValidator validator = new KingMoveValidator();
        var fromSquare = board["e1"];
        var toSquare = board["e1"];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task IsValidMove_WhenDestinationSquareHasPieceOfOppositeColor_ReturnsTrue()
    {
        var board = new Board("1N6/4P3/3b4/8/8/r5k1/8/5K2");
        var validator = new BishopMoveValidator();
        var fromSquare = board["d6"];
        var toSquare = board["b8"];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsValidMove_WhenDestinationSquareHasPieceOfSameColor_ReturnsFalse()
    {
        var board = new Board("1N6/4P3/3b4/8/8/r5k1/8/5K2");
        MoveValidator validator = new BishopMoveValidator();
        var fromSquare = board["d6"];
        var toSquare = board["a3"];

        var result = validator.IsValidMove(board, fromSquare, toSquare);

        await Assert.That(result).IsFalse();
    }
}
using Core.ChessBoard;
using Core.ChessGame;
using Core.Parsers;
using Core.Shared;
using System.Threading.Tasks;

namespace Core.Test.ChessGame;

public class ClaimDrawTest
{
    #region Threefold repetition

    [Test]
    public async Task ClaimDraw_BeforeThreefoldRepetition_ReturnsDrawByAgreement()
    {
        var game = FenParser.Parse("4k3/8/8/8/8/8/8/R3K3 w - - 0 1");
        game = game.MakeMove("e1", "d1").Game;
        game = game.MakeMove("e8", "d8").Game;
        game = game.MakeMove("d1", "e1").Game;
        game = game.MakeMove("d8", "e8").Game;

        var result = game.ClaimDraw();

        await Assert.That(result.GameResult).IsEqualTo(GameResult.DrawByAgreement);
    }

    [Test]
    public async Task ClaimDraw_AfterThreefoldRepetition_ReturnsDraw()
    {
        var game = FenParser.Parse("4k3/8/8/8/8/8/8/R3K3 w - - 0 1");
        game = game.MakeMove("e1", "d1").Game;
        game = game.MakeMove("e8", "d8").Game;
        game = game.MakeMove("d1", "e1").Game;
        game = game.MakeMove("d8", "e8").Game;
        game = game.MakeMove("e1", "d1").Game;
        game = game.MakeMove("e8", "d8").Game;
        game = game.MakeMove("d1", "e1").Game;
        game = game.MakeMove("d8", "e8").Game;

        var result = game.ClaimDraw();

        await Assert.That(result.GameResult).IsEqualTo(GameResult.DrawByThreefoldRepetition);
    }

    [Test]
    public async Task ClaimDraw_ThreeSamePositionsButDifferentCastlingRights_ReturnsDrawByAgreement()
    {
        var game = FenParser.Parse("4k3/8/8/8/8/8/8/R3K3 w Q - 0 1");
        game = game.MakeMove("e1", "d1").Game;
        game = game.MakeMove("e8", "d8").Game;
        game = game.MakeMove("d1", "e1").Game;
        game = game.MakeMove("d8", "e8").Game;
        game = game.MakeMove("e1", "d1").Game;
        game = game.MakeMove("e8", "d8").Game;
        game = game.MakeMove("d1", "e1").Game;
        game = game.MakeMove("d8", "e8").Game;

        var result = game.ClaimDraw();

        await Assert.That(result.GameResult).IsEqualTo(GameResult.DrawByAgreement);
    }

    [Test]
    public async Task ClaimDraw_AfterThreefoldRepetition_DoesNotMutateOriginalGame()
    {
        var game = FenParser.Parse("4k3/8/8/8/8/8/8/R3K3 w - - 0 1");
        game = game.MakeMove("e1", "d1").Game;
        game = game.MakeMove("e8", "d8").Game;
        game = game.MakeMove("d1", "e1").Game;
        game = game.MakeMove("d8", "e8").Game;
        game = game.MakeMove("e1", "d1").Game;
        game = game.MakeMove("e8", "d8").Game;
        game = game.MakeMove("d1", "e1").Game;
        game = game.MakeMove("d8", "e8").Game;

        var claimed = game.ClaimDraw();

        await Assert.That(game.GameResult).IsEqualTo(GameResult.InProgress);
        await Assert.That(claimed.GameResult).IsEqualTo(GameResult.DrawByThreefoldRepetition);
    }

    [Test]
    public async Task ClaimDraw_ThreefoldRepetition_WhenGameAlreadyOver_ReturnsUnchangedGame()
    {
        var game = FenParser.Parse("7k/8/6QK/8/8/8/8/8 b - - 0 1");

        await Assert.That(game.GameResult).IsEqualTo(GameResult.Stalemate);

        var result = game.ClaimDraw();

        await Assert.That(result).IsEqualTo(game);
    }

    #endregion

    #region Fifty-move rule

    [Test]
    public async Task ClaimDraw_BeforeFiftyMoveRule_ReturnsDrawByAgreement()
    {
        var game = FenParser.Parse("4k3/8/8/8/8/8/8/R3K3 w - - 99 50");

        var result = game.ClaimDraw();

        await Assert.That(result.GameResult).IsEqualTo(GameResult.DrawByAgreement);
    }

    [Test]
    public async Task ClaimDraw_AtFiftyMoveRule_ReturnsDraw()
    {
        var game = FenParser.Parse("4k3/8/8/8/8/8/8/R3K3 w - - 100 51");

        var result = game.ClaimDraw();

        await Assert.That(result.GameResult).IsEqualTo(GameResult.DrawByFiftyMoveRule);
    }

    [Test]
    public async Task ClaimDraw_BeyondFiftyMoveRule_ReturnsDraw()
    {
        var game = FenParser.Parse("4k3/8/8/8/8/8/8/R3K3 w - - 120 61");

        var result = game.ClaimDraw();

        await Assert.That(result.GameResult).IsEqualTo(GameResult.DrawByFiftyMoveRule);
    }

    [Test]
    public async Task ClaimDraw_FiftyMoveRule_WhenGameAlreadyOver_ReturnsUnchangedGame()
    {
        var game = FenParser.Parse("7k/8/6QK/8/8/8/8/8 b - - 100 60");

        await Assert.That(game.GameResult).IsEqualTo(GameResult.Stalemate);

        var result = game.ClaimDraw();

        await Assert.That(result).IsEqualTo(game);
    }

    #endregion
}

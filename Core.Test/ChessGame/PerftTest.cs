using Core.ChessBoard;
using Core.ChessGame;
using Core.Parsers;
using Core.Pieces;
using Core.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Test.ChessGame;

// See https://www.chessprogramming.org/Perft and https://www.chessprogramming.org/Perft_Results
[Category("Perft")]
public class PerftTest
{
    private static long Perft(Game game, int depth)
    {
        if (depth == 0) return 1;

        long nodes = 0;
        foreach ((Square from, Square to) in game.GetLegalMoves(game.Turn))
        {
            if (from.Piece is Pawn && (to.Rank == Rank.Eighth || to.Rank == Rank.First))
            {
                char[] promos = game.Turn == Color.White
                    ? ['Q', 'R', 'B', 'N']
                    : ['q', 'r', 'b', 'n'];
                foreach (char promo in promos)
                {
                    MakeMoveResult result = game.MakeMove(from.ToString(), to.ToString(), promo);
                    nodes += Perft(result.Game, depth - 1);
                }
            }
            else
            {
                MakeMoveResult result = game.MakeMove(from.ToString(), to.ToString());
                nodes += Perft(result.Game, depth - 1);
            }
        }
        return nodes;
    }

    private static Dictionary<string, long> Divide(Game game, int depth)
    {
        var result = new Dictionary<string, long>();
        foreach ((Square from, Square to) in game.GetLegalMoves(game.Turn))
        {
            if (from.Piece is Pawn && (to.Rank == Rank.Eighth || to.Rank == Rank.First))
            {
                char[] promos = game.Turn == Color.White
                    ? ['Q', 'R', 'B', 'N']
                    : ['q', 'r', 'b', 'n'];
                foreach (char promo in promos)
                {
                    string key = $"{from}{to}{char.ToLower(promo)}";
                    MakeMoveResult moveResult = game.MakeMove(from.ToString(), to.ToString(), promo);
                    result[key] = Perft(moveResult.Game, depth - 1);
                }
            }
            else
            {
                string key = $"{from}{to}";
                MakeMoveResult moveResult = game.MakeMove(from.ToString(), to.ToString());
                result[key] = Perft(moveResult.Game, depth - 1);
            }
        }
        return result;
    }

    [Test]
    [Arguments(1, 20L)]
    [Arguments(2, 400L)]
    [Arguments(3, 8_902L)]
    [Arguments(4, 197_281L)]
    public async Task Perft_StartingPosition(int depth, long expected)
    {
        var game = new Game();

        long nodes = Perft(game, depth);

        await Assert.That(nodes).IsEqualTo(expected);
    }

    // Position 2 is also known as Kiwipete
    [Test]
    [Arguments(1, 48L)]
    [Arguments(2, 2_039L)]
    [Arguments(3, 97_862L)]
    [Arguments(4, 4_085_603L)]
    public async Task Perft_Position2(int depth, long expected)
    {
        Game game = FenParser.Parse("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");

        long nodes = Perft(game, depth);

        await Assert.That(nodes).IsEqualTo(expected);
    }

    // Specifically stresses en passant, pins through the king, and promotion.
    [Test]
    [Arguments(1, 14L)]
    [Arguments(2, 191L)]
    [Arguments(3, 2_812L)]
    [Arguments(4, 43_238L)]
    [Arguments(5, 674_624L)]
    public async Task Perft_Position3(int depth, long expected)
    {
        Game game = FenParser.Parse("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");

        long nodes = Perft(game, depth);

        await Assert.That(nodes).IsEqualTo(expected);
    }

    // Stresses promotions (both sides), castling with only one rook, and captures near the king.
    [Test]
    [Arguments(1, 6L)]
    [Arguments(2, 264L)]
    [Arguments(3, 9_467L)]
    [Arguments(4, 422_333L)]
    public async Task Perft_Position4(int depth, long expected)
    {
        Game game = FenParser.Parse("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1");

        long nodes = Perft(game, depth);

        await Assert.That(nodes).IsEqualTo(expected);
    }

    // Stresses promotions with check and discovered checks.
    [Test]
    [Arguments(1, 44L)]
    [Arguments(2, 1_486L)]
    [Arguments(3, 62_379L)]
    [Arguments(4, 2_103_487L)]
    public async Task Perft_Position5(int depth, long expected)
    {
        Game game = FenParser.Parse("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8");

        long nodes = Perft(game, depth);

        await Assert.That(nodes).IsEqualTo(expected);
    }

    // Symmetric position; good general stress test with no castling rights.
    [Test]
    [Arguments(1, 46L)]
    [Arguments(2, 2_079L)]
    [Arguments(3, 89_890L)]
    [Arguments(4, 3_894_594L)]
    public async Task Perft_Position6(int depth, long expected)
    {
        Game game = FenParser.Parse("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10");

        long nodes = Perft(game, depth);

        await Assert.That(nodes).IsEqualTo(expected);
    }
}

using System.Threading.Tasks;
using Core.ChessBoard;
using Core.Exceptions;

namespace Core.Test.ChessBoard;

public class RankTest
{
    [Test]
    public async Task ParseChar_WithValidChar_ReturnsRank()
    {
        var actual = Rank.ParseChar('2');

        await Assert.That(actual).IsEqualTo(Rank.Second);
    }

    [Test]
    public async Task ParseChar_WithInvalidChar_ThrowsOutOfBoardException()
    {
        void Act() => Rank.ParseChar('0');

        var exception = await Assert.That(Act)
            .Throws<OutOfBoardException>();
        await Assert.That(exception.Message).IsEqualTo("Rank -1 is out of board (must be between 0 and 7).");
    }

    [Test]
    [Arguments(5)] // Up
    [Arguments(1)] // Down
    public async Task DistanceTo_OtherRank_ReturnsExpectedDistance(int rankValue)
    {
        var rank = (Rank) rankValue;

        var actual = Rank.Fourth.DistanceTo(rank);

        await Assert.That(actual).IsEqualTo(2);
    }

    [Test]
    public async Task Increment_WhenRankIsEighth_ThrowsOutOfBoardException()
    {
        void Act()
        {
            #pragma warning disable S1854
            var rank = Rank.Eighth;
            _ = ++rank;
            #pragma warning restore S1854
        }

        var exception = await Assert.That(Act)
            .Throws<OutOfBoardException>();
        await Assert.That(exception.Message).IsEqualTo("Rank 8 is out of board (must be between 0 and 7).");
    }

    [Test]
    public async Task Decrement_WhenRankIsFirst_ThrowsOutOfBoardException()
    {
        void Act()
        {
            #pragma warning disable S1854
            var rank = Rank.First;
            _ = --rank;
            #pragma warning restore S1854
        }

        var exception = await Assert.That(Act)
            .Throws<OutOfBoardException>();
        await Assert.That(exception.Message).IsEqualTo("Rank -1 is out of board (must be between 0 and 7).");
    }
}
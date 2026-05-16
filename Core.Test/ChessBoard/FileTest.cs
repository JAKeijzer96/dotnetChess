using System.Threading.Tasks;
using Core.ChessBoard;
using Core.Exceptions;

namespace Core.Test.ChessBoard;

public class FileTest
{
    [Test]
    public async Task ParseChar_WithValidChar_ReturnsFile()
    {
        var actual = File.ParseChar('c');

        await Assert.That(actual).IsEqualTo(File.C);
    }

    [Test]
    public async Task ParseChar_WithInvalidChar_ThrowsOutOfBoardException()
    {
        void Act() => File.ParseChar('1');

        var exception = await Assert.That(Act).Throws<OutOfBoardException>();
        await Assert.That(exception!.Message).IsEqualTo("File 8 is out of board (must be between 0 and 7).");
    }

    [Test]
    [Arguments(5)] // To the right
    [Arguments(1)] // To the left
    public async Task DistanceTo_OtherFile_ReturnsExpectedDistance(int fileValue)
    {
        var file = (File) fileValue;

        var actual = File.D.DistanceTo(file);

        await Assert.That(actual).IsEqualTo(2);
    }

    [Test]
    public async Task Increment_WhenFileIsH_ThrowsOutOfBoardException()
    {
        void Act()
        {
            #pragma warning disable S1854
            var file = File.H;
            _ = ++file;
            #pragma warning restore S1854
        }
        
        var exception = await Assert.That(Act).Throws<OutOfBoardException>();
        await Assert.That(exception!.Message).IsEqualTo("File 8 is out of board (must be between 0 and 7).");
    }

    [Test]
    public async Task Decrement_WhenFileIsA_ThrowsOutOfBoardException()
    {
        void Act()
        {
            #pragma warning disable S1854
            var file = File.A;
            _ = --file;
            #pragma warning restore S1854
        }

        var exception = await Assert.That(Act).Throws<OutOfBoardException>();
        await Assert.That(exception!.Message).IsEqualTo("File -1 is out of board (must be between 0 and 7).");
    }
}
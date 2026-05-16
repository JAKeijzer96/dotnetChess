using System;
using System.Threading.Tasks;
using Core.Pieces;
using Core.Shared;

namespace Core.Test.Pieces;

public class PieceFactoryTest
{
    [Test]
    [Arguments('K', typeof(King), Color.White)]
    [Arguments('Q', typeof(Queen), Color.White)]
    [Arguments('R', typeof(Rook), Color.White)]
    [Arguments('B', typeof(Bishop), Color.White)]
    [Arguments('N', typeof(Knight), Color.White)]
    [Arguments('P', typeof(Pawn), Color.White)]
    [Arguments('k', typeof(King), Color.Black)]
    [Arguments('q', typeof(Queen), Color.Black)]
    [Arguments('r', typeof(Rook), Color.Black)]
    [Arguments('b', typeof(Bishop), Color.Black)]
    [Arguments('n', typeof(Knight), Color.Black)]
    [Arguments('p', typeof(Pawn), Color.Black)]
    public async Task CreatePiece_CreatesPieceWithCorrectTypeAndColor(char pieceChar, Type expectedType, Color expectedColor)
    {
        var result = PieceFactory.CreatePiece(pieceChar);

        await Assert.That(result).IsOfType(expectedType);
        await Assert.That(result.Color).IsEqualTo(expectedColor);
    }
    
    [Test]
    public async Task CreatePiece_WithIncorrectChar_ThrowsArgumentException()
    {
        var exception = await Assert.That(() => PieceFactory.CreatePiece('V'))
            .Throws<ArgumentException>();
        await Assert.That(exception.Message).IsEqualTo("Invalid piece character: 'V'");
    }
}
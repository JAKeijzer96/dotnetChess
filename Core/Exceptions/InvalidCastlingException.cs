using Core.ChessBoard;
using File = Core.ChessBoard.File;

namespace Core.Exceptions;

public class InvalidCastlingException : Exception
{
    public InvalidCastlingException()
    {
    }

    public InvalidCastlingException(string? message) : base(message)
    {
    }

    public InvalidCastlingException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public InvalidCastlingException(Square from, Square to, File blockedFile) : base(
        $"Cannot castle from {from} to {to} because there is a piece blocking on file {blockedFile}")
    {
    }
}
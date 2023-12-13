using Core.ChessBoard;
using File = Core.ChessBoard.File;

namespace Core.Exceptions;

public class InvalidCastlingMoveException : Exception
{
    public InvalidCastlingMoveException()
    {
    }

    public InvalidCastlingMoveException(string? message) : base(message)
    {
    }

    public InvalidCastlingMoveException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public InvalidCastlingMoveException(Square from, Square to, File blockedFile) : base(
        $"Cannot castle from {from} to {to} because there is a piece blocking on file {blockedFile}")
    {
    }
}
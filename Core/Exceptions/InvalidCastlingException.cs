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
}
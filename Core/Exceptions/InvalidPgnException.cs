namespace Core.Exceptions;

public class InvalidPgnException : Exception
{
    public InvalidPgnException()
    {
    }

    public InvalidPgnException(string? message) : base(message)
    {
    }

    public InvalidPgnException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

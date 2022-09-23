namespace Core.Exceptions;

public class InvalidFenException : Exception
{
    public InvalidFenException()
    {
    }

    public InvalidFenException(string message) : base(message)
    {
    }

    public InvalidFenException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
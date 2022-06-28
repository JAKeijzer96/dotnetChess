namespace Core.Exceptions;

public class OutOfBoardException : Exception
{
    public OutOfBoardException()
    {
    }

    public OutOfBoardException(string message) : base(message)
    {
    }

    public OutOfBoardException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
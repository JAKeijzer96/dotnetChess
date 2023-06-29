namespace Core.Exceptions;

public class InvalidPromotionException : Exception
{
    public InvalidPromotionException()
    {
    }

    public InvalidPromotionException(string? message) : base(message)
    {
    }

    public InvalidPromotionException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
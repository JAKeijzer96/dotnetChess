namespace Core.Exceptions
{
    public class InvalidColorException : Exception
    {
        public InvalidColorException()
        {
        }

        public InvalidColorException(string? message) : base(message)
        {
        }

        public InvalidColorException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}

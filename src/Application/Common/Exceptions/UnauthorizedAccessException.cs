namespace Application.Common.Exceptions;

public class UnauthorizedAccessException : Exception
{
    public UnauthorizedAccessException() : base("User is not authenticated") { }

    public UnauthorizedAccessException(string message) : base(message) { }
    public UnauthorizedAccessException(string message, Exception innerException)
        : base(message, innerException) { }
}


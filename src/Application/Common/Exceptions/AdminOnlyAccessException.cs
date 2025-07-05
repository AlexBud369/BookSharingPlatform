namespace Application.Common.Exceptions;

public class AdminOnlyAccessException : Exception
{
    public AdminOnlyAccessException() : base("Only admins can perfom this action") { }

    public AdminOnlyAccessException(string message) : base(message) { }
    public AdminOnlyAccessException(string message, Exception innerException)
        : base(message, innerException) { }
}


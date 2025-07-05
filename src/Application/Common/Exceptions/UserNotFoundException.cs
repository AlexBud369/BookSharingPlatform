namespace Application.Common.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException() { }

    public UserNotFoundException(string message) : base(message) { }
    public UserNotFoundException(string message, Exception innerException)
        :base(message, innerException) { }

    public UserNotFoundException(Guid userId) 
        : base($"User with Id {userId} was not found") { }

}


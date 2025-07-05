namespace Application.Common.Exceptions;

public class BookNotFoundException : Exception
{
    public BookNotFoundException() { }

    public BookNotFoundException(string message) : base(message) { }
    public BookNotFoundException(string message, Exception innerException)
        : base(message, innerException) { }

    public BookNotFoundException(Guid bookId)
        : base($"Book with Id {bookId} was not found") { }

}


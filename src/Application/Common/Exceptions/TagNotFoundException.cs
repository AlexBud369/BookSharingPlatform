namespace Application.Common.Exceptions;

public class TagNotFoundException : Exception
{
    public TagNotFoundException() { }

    public TagNotFoundException(string message) : base(message) { }
    public TagNotFoundException(string message, Exception innerException)
        : base(message, innerException) { }

    public TagNotFoundException(Guid tagId)
        : base($"Tag with Id {tagId} was not found") { }

}


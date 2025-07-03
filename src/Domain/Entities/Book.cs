using System.Data;

namespace Domain.Entities;

public class Book
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; private set; }
    public string Author { get; private set; }
    public string? Description { get; private set; }
    public string? CoverImageUrl { get; private set; }
    public Guid CreatedByUserId { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public IReadOnlyList<Tag> Tags { get => TagsList.AsReadOnly(); }
    private readonly List<Tag> TagsList = new();

    private Book() { }

    public Book(string title, string author, Guid createdByUserId, string? description = null, string? coverImageUrl = null)
    {
        Title = title;
        Author = author;
        Description = description;
        CoverImageUrl = coverImageUrl;
        CreatedByUserId = createdByUserId;
    }
}


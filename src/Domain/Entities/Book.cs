using System.Data;

namespace Domain.Entities;

public class Book
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public IReadOnlyList<Tag> Tags { get => TagsList.AsReadOnly(); }
    private readonly List<Tag> TagsList = new();

}


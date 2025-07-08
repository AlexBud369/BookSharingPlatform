using System.Data;

namespace Domain.Entities;
public class User
{
    public Guid userId { get; init; } = Guid.NewGuid();
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public Role Role { get; set; }
    public bool IsBlocked { get; set; } = false;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public IReadOnlyList<RefreshToken> RefreshTokens { get => RefreshTokensList.AsReadOnly(); }
    public IReadOnlyList<Book> Books { get => BooksList.AsReadOnly(); }
    private readonly List<RefreshToken> RefreshTokensList = new();
    private readonly List<Book> BooksList = new();

    private User() { }

}

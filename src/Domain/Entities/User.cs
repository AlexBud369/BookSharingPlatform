using System.Data;

namespace Domain.Entities;
public class User
{
    public Guid userId { get; init; } = Guid.NewGuid();
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public Role Role { get; private set; }
    public bool IsBlocked { get; private set; } = false;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public IReadOnlyList<RefreshToken> RefreshTokens { get => RefreshTokensList.AsReadOnly(); }
    public IReadOnlyList<Book> Books { get => BooksList.AsReadOnly(); }
    private readonly List<RefreshToken> RefreshTokensList = new();
    private readonly List<Book> BooksList = new();

    private User()
    {

    }

    public User(string username, string email, string passwordHash, Role role = Role.User)
    {
        userId = Guid.NewGuid();
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        
    }

}

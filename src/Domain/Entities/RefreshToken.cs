namespace Domain.Entities;

/// <summary>
/// The entity of an update token is used to extend user
/// authentication sessions on a book-sharing platform.
/// </summary>
public class RefreshToken
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Token { get; init; }
    public Guid UserId { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; init; }
    public bool IsRevoked { get; set; }

    private RefreshToken() { }


}


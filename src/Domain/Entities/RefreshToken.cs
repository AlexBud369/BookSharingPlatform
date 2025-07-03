namespace Domain.Entities
{
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
        public bool IsRevoked { get; private set; }

        private RefreshToken() { } 

        public RefreshToken(string token, Guid userId, DateTime expiresAt)
        {
            Token = token;
            UserId = userId;
            ExpiresAt = expiresAt;
        }

        public void Revoke()
        {
            if (IsRevoked)
            {
                throw new InvalidOperationException("Token is already revoked.");
            }
            IsRevoked = true;
        }
    }
}

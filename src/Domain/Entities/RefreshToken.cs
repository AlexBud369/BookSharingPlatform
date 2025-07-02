namespace Domain.Entities
{
    /// <summary>
    /// The entity of an update token is used to extend user
    /// authentication sessions on a book-sharing platform.
    /// </summary>
    public class RefreshToken
    {
        private Guid tokenId;
        private string token;
        private Guid userId;
        private DateTime createdAt;
        private DateTime expiresAt;
        private bool isRevoked;

        private RefreshToken() { } 

        public RefreshToken(string token, Guid userId, DateTime expiresAt)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be empty.", nameof(token));
            tokenId = Guid.NewGuid();
            this.token = token;
            this.userId = userId;
            createdAt = DateTime.UtcNow;
            this.expiresAt = expiresAt;
            isRevoked = false;
        }

        public Guid Id { get { return tokenId; } }
        public string Token { get { return token; } }
        public Guid UserId { get { return userId; } }
        public DateTime CreatedAt { get { return createdAt; } }
        public DateTime ExpiresAt { get { return expiresAt; } }
        public bool IsRevoked { get { return isRevoked; } }


        public void Revoke()
        {
            if (isRevoked)
            {
                throw new InvalidOperationException("Token is already revoked.");
            }
            isRevoked = true;
        }
    }
}

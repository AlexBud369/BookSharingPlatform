using System.Data;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;

namespace Domain.Entities
{
    public class User
    {
        private Guid userId;
        private string username;
        private string email;
        private string passwordHash;
        private Role role;
        private bool isBlocked;
        private DateTime createdAt;
        private ICollection<RefreshToken> refreshTokens;
        private readonly ICollection<Book> books;

        private User()
        {
            RefreshTokens = new List<RefreshToken>();
            Books = new List<Book>();
        }

        public User(string username, string email, string passwordHash, Role role = Role.User)
        {
            ValidateNotEmpty(username, nameof(username));
            ValidateNotEmpty(email, nameof(email));
            ValidateNotEmpty(passwordHash, nameof(passwordHash));

            userId = Guid.NewGuid();
            this.username = username;
            this.email = email;
            this.passwordHash = passwordHash;
            this.role = role;
            isBlocked = false;
            createdAt = DateTime.UtcNow;
            refreshTokens = new List<RefreshToken>();
            books = new List<Book>();
        }

        public Guid UserId
        {
            get { return userId; }
        }

        public string Name
        {
            get { return username; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Name cannot be empty.", nameof(value));
                }
                username = value;
            }
        }

        public string Email
        {
            get { return email; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Email cannot be empty.", nameof(value));
                }
                email = value;
            }
        }

        public string PasswordHash
        {
            get { return passwordHash; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Password hash cannot be empty.", nameof(value));
                }
                passwordHash = value;
            }
        }

        public Role Role
        {
            get { return role; }
            private set { role = value; }
        }

        public bool IsBlocked
        {
            get { return isBlocked; }
            private set { isBlocked = value; }
        }

        public DateTime CreatedAt
        {
            get { return createdAt; }
        }

        public ICollection<RefreshToken> RefreshTokens
        {
            get { return refreshTokens; }
            private set { refreshTokens = value ?? new List<RefreshToken>(); }
        }


        public void UpdateName(string newName)
        {
            Name = newName;
        }

        public void UpdateEmail(string newEmail)
        {
            Email = newEmail;
        }

        public void UpdatePasswordHash(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
        }

        public void ChangeRole(Role newRole)
        {
            Role = newRole;
        }

        public void Block()
        {
            if (isBlocked)
            {
                throw new InvalidOperationException("User is already blocked.");
            }
            IsBlocked = true;
        }

        public void Unblock()
        {
            if (!isBlocked)
            {
                throw new InvalidOperationException("User is not blocked.");
            }
            IsBlocked = false;
        }

        public void AddBook(Book book)
        {
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }
            books.Add(book);
        }

        public void AddRefreshToken(RefreshToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            refreshTokens.Add(token);
        }

        private void ValidateNotEmpty(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} cannot be empty.", paramName);
        }

    }
}

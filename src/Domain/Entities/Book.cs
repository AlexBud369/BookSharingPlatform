namespace Domain.Entities
{
    public class Book
    {
        private Guid bookId;
        private string title;
        private string author;
        private string? description;
        private string? coverImageUrl;
        private Guid createdByUserId;
        private DateTime createdAt;
        private readonly List<Tag> tags;

        private Book()
        {
            tags = new List<Tag>();
        }

        public Book(string title, string author, Guid createdByUserId, string? description = null, string? coverImageUrl = null)
        {
            ValidateNotEmpty(title, nameof(title));
            ValidateNotEmpty(author, nameof(author));

            bookId = Guid.NewGuid();
            this.title = title;
            this.author = author;
            this.description = description;
            this.coverImageUrl = coverImageUrl;
            this.createdByUserId = createdByUserId;
            createdAt = DateTime.UtcNow;
            tags = new List<Tag>();
        }

        public Guid Id
        {
            get { return bookId; }
        }

        public string Title
        {
            get { return title; }
            private set
            {
                ValidateNotEmpty(value, nameof(Title));
                title = value;
            }
        }

        public string Author
        {
            get { return author; }
            private set
            {
                ValidateNotEmpty(value, nameof(Author));
                author = value;
            }
        }

        public string? Description
        {
            get { return description; }
            private set { description = value; }
        }

        public string? CoverImageUrl
        {
            get { return coverImageUrl; }
            private set { coverImageUrl = value; }
        }

        public Guid CreatedByUserId
        {
            get { return createdByUserId; }
        }

        public DateTime CreatedAt
        {
            get { return createdAt; }
        }

        public IReadOnlyList<Tag> Tags
        {
            get { return tags.AsReadOnly(); }
        }

        public void UpdateDetails(string title, string author, string? description)
        {
            Title = title;
            Author = author;
            Description = description;
        }

        public void UpdateCoverImageUrl(string? coverImageUrl)
        {
            CoverImageUrl = coverImageUrl;
        }

        public void AddTag(Tag tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));
            if (!tags.Contains(tag))
                tags.Add(tag);
        }

        public void RemoveTag(Tag tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));
            tags.Remove(tag);
        }

        public bool CanBeDeletedBy(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            return user.UserId == createdByUserId || user.Role == Role.Admin;
        }

        private void ValidateNotEmpty(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} cannot be empty.", paramName);
        }

    }
}

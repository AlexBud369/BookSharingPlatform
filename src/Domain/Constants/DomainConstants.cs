using Domain.Enums;

namespace Domain.Constants;

public static class DomainConstants
{
    public static class Book
    {
        public const int TitleMaxLength = 200;
        public const int AuthorMaxLength = 100;
        public const int DescriptionMaxLength = 1500;
        public const int MinPageNumber = 1;
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;
        public const int SearchQueryMaxLength = 200;
    }

    public static class Tag
    {
        public const int NameMaxLength = 50;
        public const int MinPageNumber = 1;
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;
    }

    public static class User
    {
        public const int EmailMaxLength = 100;
        public const int UsernameMaxLength = 50;
        public const int PasswordMinLength = 6;
        public const int RefreshTokenMaxLength = 500;
        public const int MinPageNumber = 1;
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;
        public static readonly string[] AllowedRoles = Enum.GetNames<UserRole>();
    }

    public static class BookCover
    {
        public const int UrlMaxLength = 500;
        public static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png" };
        public const long MaxFileSizeBytes = 5 * 1024 * 1024;
    }
}

using Azure;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Book>()
            .HasMany(b => b.Tags)
            .WithMany(t => t.Books);

        builder.Entity<Book>()
            .HasOne(b => b.CreatedByUser)
            .WithMany(u => u.Books)
            .HasForeignKey(b => b.CreatedByUserId);

        builder.Entity<Book>()
            .Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(DomainConstants.Book.TitleMaxLength);

        builder.Entity<Book>()
            .Property(b => b.Author)
            .IsRequired()
            .HasMaxLength(DomainConstants.Book.AuthorMaxLength);

        builder.Entity<Book>()
            .Property(b => b.CoverImageUrl)
            .IsRequired(false)
            .HasMaxLength(DomainConstants.BookCover.UrlMaxLength);

        builder.Entity<Tag>()
            .Property(t => t.TagName)
            .IsRequired()
            .HasMaxLength(DomainConstants.Tag.NameMaxLength);

        builder.Entity<ApplicationUser>()
            .Property(u => u.IsBlocked)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Entity<ApplicationUser>()
            .Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId);
    }
}
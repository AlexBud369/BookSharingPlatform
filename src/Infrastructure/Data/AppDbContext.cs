using Azure;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;
public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Tag> Tags { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Book>()
            .HasMany(b => b.Tags)
            .WithMany(t => t.Books)
            .UsingEntity(j => j.ToTable("BookTags"));

        builder.Entity<Book>()
            .HasOne(b => b.CreatedByUser)
            .WithMany(u => u.Books)
            .HasForeignKey(b => b.CreatedByUserId);

        builder.Entity<Book>()
            .Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Entity<Book>()
            .Property(b => b.Author)
            .IsRequired()
            .HasMaxLength(100);

        builder.Entity<Tag>()
            .Property(t => t.tagName)
            .IsRequired()
            .HasMaxLength(50);
    }
}
}

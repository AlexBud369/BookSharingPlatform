using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class User
{
    public Guid userId { get; init; } = Guid.NewGuid();
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public bool IsBlocked { get; private set; } = false;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public IReadOnlyList<Book> Books { get => BooksList.AsReadOnly(); }
    private readonly List<Book> BooksList = new();

    private User() { }

}
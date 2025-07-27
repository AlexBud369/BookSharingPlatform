using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public bool IsBlocked { get; set; }
    public IReadOnlyList<Book> Books { get => BooksList.AsReadOnly(); }
    public IReadOnlyList<RefreshToken> RefreshTokens { get => RefreshTokensList.AsReadOnly(); }
    private readonly List<Book> BooksList = new();
    private readonly List<RefreshToken> RefreshTokensList = new();

}
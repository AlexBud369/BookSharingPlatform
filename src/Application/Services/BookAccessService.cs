using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Persistence.Data;

namespace Application.Services;

public class BookAccessService : IBookAccessService
{
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookAccessService(
        IStringLocalizer<SharedResources> localizer,
        AppDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _localizer = localizer;
        _context = context;
        _userManager = userManager;
        Guard.Initialize(_localizer);
    }

    public async Task ValidateBookAccessAsync(Book book, Guid userId, bool isAdmin, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(book, nameof(book), _localizer.GetString(SharedResources.BookNotFound));
        Guard.AgainstEmptyGuid(book.Id, nameof(book.Id), _localizer.GetString(SharedResources.BookIdRequired));
        Guard.AgainstEmptyGuid(userId, nameof(userId), _localizer.GetString(SharedResources.UserIdRequired));
        Guard.AgainstUnauthorized(book.CreatedByUserId == userId || isAdmin, _localizer.GetString(SharedResources.UnauthorizedAccess));
    }

    public async Task<bool> CanDeleteBookAsync(Guid userId, Guid bookId, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(userId, nameof(userId), _localizer.GetString(SharedResources.UserIdRequired));
        Guard.AgainstEmptyGuid(bookId, nameof(bookId), _localizer.GetString(SharedResources.BookIdRequired));

        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId, cancellationToken);
        if (book == null) { 
            return false;
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null) {
            return false;
        }

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        return book.CreatedByUserId == userId || isAdmin;
    }

    public async Task<bool> CanEditBookAsync(Guid userId, Guid bookId, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(userId, nameof(userId), _localizer.GetString(SharedResources.UserIdRequired));
        Guard.AgainstEmptyGuid(bookId, nameof(bookId), _localizer.GetString(SharedResources.BookIdRequired));

        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId, cancellationToken);
        if (book == null) {
            return false;
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null) {
            return false;
        }

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        return book.CreatedByUserId == userId || isAdmin;
    }
}
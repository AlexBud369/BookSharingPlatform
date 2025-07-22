using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Localization;

namespace Application.Services;

public class BookAccessService : IBookAccessService
{
    private readonly IStringLocalizer<SharedResource> _localizer;

    public BookAccessService(IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    public Task ValidateBookAccessAsync(Book book, Guid userId, bool isAdmin, CancellationToken cancellationToken)
    {
        Guard.AgainstUnauthorized(book.CreatedByUserId == userId || isAdmin, "UnauthorizedAccess");
        return Task.CompletedTask;
    }
}

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
    private readonly IStringLocalizer<SharedResources> _localizer;

    public BookAccessService(IStringLocalizer<SharedResources> localizer)
    {
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    public Task ValidateBookAccessAsync(Book book, Guid userId, bool isAdmin, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(book, nameof(book), _localizer.GetString(SharedResources.BookNotFound));
        Guard.AgainstEmptyGuid(book.Id, nameof(book.Id), _localizer.GetString(SharedResources.BookIdRequired));
        Guard.AgainstEmptyGuid(userId, nameof(userId), _localizer.GetString(SharedResources.UserIdRequired));
        Guard.AgainstUnauthorized(book.CreatedByUserId == userId || isAdmin, _localizer.GetString(SharedResources.UnauthorizedAccess));

        return Task.CompletedTask;
    }
}
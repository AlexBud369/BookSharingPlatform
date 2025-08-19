using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces;

public interface IBookAccessService
{
    Task ValidateBookAccessAsync(Book book, Guid userId, bool isAdmin, CancellationToken cancellationToken);
    Task<bool> CanDeleteBookAsync(Guid userId, Guid bookId, CancellationToken cancellationToken);
    Task<bool> CanEditBookAsync(Guid userId, Guid bookId, CancellationToken cancellationToken);
}

using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces;

public interface IBookAccessService
{
    Task ValidateBookAccessAsync(Book book, Guid userId, bool isAdmin, CancellationToken cancellationToken);
}

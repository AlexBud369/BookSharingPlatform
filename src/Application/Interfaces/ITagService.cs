using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces;

public interface ITagService
{
    Task AddTagsToBookAsync(Book book, IEnumerable<string>? tagNames, CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetTagNamesByIdsAsync(IEnumerable<Guid> tagIds, CancellationToken cancellationToken);
    Task UpdateTagsForBookAsync(Book book, IEnumerable<string> tagNames, CancellationToken cancellationToken);
}
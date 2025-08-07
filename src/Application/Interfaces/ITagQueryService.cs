using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.Tag;


namespace Application.Interfaces;

public interface ITagQueryService
{
    Task<IEnumerable<TagDto>> GetTagsAsync(
        int pageNumber,
        int pageSize,
        string? tagName,
        CancellationToken cancellationToken);
    Task<TagDto> GetTagByIdAsync(Guid tagId, CancellationToken cancellationToken);
}


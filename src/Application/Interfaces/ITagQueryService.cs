using Application.DTOs;
using Application.DTOs.Tag;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface ITagQueryService
{
    Task<PagedResponseDto<TagDto>> GetTagsAsync(
        int pageNumber,
        int pageSize,
        string? tagName,
        CancellationToken cancellationToken);
    Task<TagDto> GetTagByIdAsync(Guid tagId, CancellationToken cancellationToken);
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.DTOs.Tag;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Application.Services;

public class TagQueryService : ITagQueryService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public TagQueryService(
        AppDbContext context,
        IMapper mapper,
        IStringLocalizer<SharedResource> localizer)
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    public async Task<IEnumerable<TagDto>> GetTagsAsync(int pageNumber, int pageSize, string? tagName, CancellationToken cancellationToken)
    {
        Guard.Against(pageNumber >= 1, nameof(pageNumber), "InvalidPageNumber");
        Guard.Against(pageSize >= 1, nameof(pageSize), "InvalidPageSize");

        var query = _context.Tags.AsQueryable();

        if (!string.IsNullOrEmpty(tagName)) {
            query = query.Where(t => t.tagName.Contains(tagName));
        }

        var tags = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<TagDto>>(tags);
    }

    public async Task<TagDto> GetTagByIdAsync(Guid tagId, CancellationToken cancellationToken)
    {
        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.tagId == tagId, cancellationToken);
        Guard.AgainstNull(tag, nameof(tagId), "TagNotFound", tagId.ToString());
        return _mapper.Map<TagDto>(tag);
    }
}
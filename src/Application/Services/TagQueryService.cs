using System;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.DTOs.Tag;
using Application.Interfaces;
using Domain.Entities;
using Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Application.Services;

public class TagQueryService : ITagQueryService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public TagQueryService(
        AppDbContext context,
        IMapper mapper,
        IStringLocalizer<SharedResources> localizer)
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    public async Task<IEnumerable<TagDto>> GetTagsAsync(int pageNumber, int pageSize, string? tagName, CancellationToken cancellationToken)
    {
        Guard.AgainstInvalidPageNumber(pageNumber, nameof(pageNumber), _localizer.GetString(SharedResources.InvalidPageNumber));
        Guard.AgainstInvalidPageSize(pageSize, nameof(pageSize), _localizer.GetString(SharedResources.InvalidPageSize));

        var query = _context.Tags.AsQueryable();

        if (!string.IsNullOrEmpty(tagName)) {
            query = query.Where(t => t.TagName.Contains(tagName));
        }

        var tags = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<TagDto>>(tags);
    }

    public async Task<TagDto> GetTagByIdAsync(Guid tagId, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(tagId, nameof(tagId), _localizer.GetString(SharedResources.TagIdRequired));

        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.TagId == tagId, cancellationToken);
        Guard.AgainstNull(tag, nameof(tagId), _localizer.GetString(SharedResources.TagNotFound), tagId.ToString());
        
        return _mapper.Map<TagDto>(tag);
    }
}
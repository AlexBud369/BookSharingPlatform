using Application.Common;
using Application.DTOs;
using Application.DTOs.Tag;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Persistence.Data;

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

    public async Task<PagedResponseDto<TagDto>> GetTagsAsync(int pageNumber, int pageSize, string? tagName, CancellationToken cancellationToken)
    {
        Guard.AgainstInvalidPageNumber(pageNumber, nameof(pageNumber), _localizer.GetString(SharedResources.InvalidPageNumber));
        Guard.AgainstInvalidPageSize(pageSize, nameof(pageSize), _localizer.GetString(SharedResources.InvalidPageSize));

        var query = _context.Tags.AsQueryable();

        if (!string.IsNullOrEmpty(tagName)) {
            query = query.Where(t => t.TagName.Contains(tagName));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var tags = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponseDto<TagDto>
        {
            Items = _mapper.Map<IEnumerable<TagDto>>(tags),
            TotalItems = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
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
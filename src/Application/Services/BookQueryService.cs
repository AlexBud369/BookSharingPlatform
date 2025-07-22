using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.DTOs;
using Application.DTOs.Book;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Application.Services;

public class BookQueryService : IBookQueryService
{
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public BookQueryService(
        IStringLocalizer<SharedResource> localizer,
        AppDbContext context,
        IMapper mapper)
    {
        Guard.AgainstNull(localizer, nameof(localizer), "LocalizerRequired");
        Guard.AgainstNull(context, nameof(context), "DbContextRequired");
        Guard.AgainstNull(mapper, nameof(mapper), "MapperRequired");

        _localizer = localizer;
        _context = context;
        _mapper = mapper;
        Guard.Initialize(_localizer);
    }

    public async Task<PagedResult<BookDto>> GetBooksAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm,
        string? tag,
        Guid? createdByUserId,
        string? sortBy,
        bool sortDescending,
        CancellationToken cancellationToken)
    {
       
        var query = _context.Books
            .Include(b => b.Tags)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm)) {
            var searchLower = searchTerm.ToLower();
            query = query.Where(b => b.Title.ToLower().Contains(searchLower) ||
                                    b.Author.ToLower().Contains(searchLower));
        }

        if (tagIds != null && tagIds.Any()) {
            query = query.Where(b => b.Tags.Any(t => tagIds.Contains(t.Id)));
        }

        if (createdByUserId.HasValue) {
            query = query.Where(b => b.CreatedByUserId == createdByUserId.Value);
        }

        query = sortBy switch
        {
            "title" => sortDescending
                ? query.OrderByDescending(b => b.Title)
                : query.OrderBy(b => b.Title),
            "createdAt" => sortDescending
                ? query.OrderByDescending(b => b.CreatedAt)
                : query.OrderBy(b => b.CreatedAt),
            _ => query.OrderBy(b => b.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<BookDto>
        {
            Items = _mapper.Map<List<BookDto>>(items),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<BookDto> GetBookByIdAsync(Guid bookId, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .Include(b => b.Tags)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == bookId, cancellationToken);
        Guard.AgainstNull(book, nameof(bookId), "BookNotFound", bookId.ToString());

        return _mapper.Map<BookDto>(book);
    }
}
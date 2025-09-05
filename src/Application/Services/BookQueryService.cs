using Application.Common;
using Application.Common.Enums;
using Application.DTOs;
using Application.DTOs.Book;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Persistence.Data;

namespace Application.Services;

public class BookQueryService : IBookQueryService
{
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public BookQueryService(
        IStringLocalizer<SharedResources> localizer,
        AppDbContext context,
        IMapper mapper)
    {
        _localizer = localizer;
        _context = context;
        _mapper = mapper;
        Guard.Initialize(_localizer);
    }

    public async Task<PagedResponseDto<BookDto>> GetBooksAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm,
        IEnumerable<Guid>? tagIds,
        Guid? createdByUserId,
        string? sortBy,
        bool sortDescending,
        CancellationToken cancellationToken)
    {
        var query = BuildBaseQuery();
        query = ApplyFilters(query, searchTerm, tagIds, createdByUserId);
        query = ApplySorting(query, sortBy, sortDescending);

        var totalItems = await GetTotalCountAsync(query, cancellationToken);
        var items = await GetPagedItemsAsync(query, pageNumber, pageSize, cancellationToken);

        return CreatePagedResult(items, totalItems, pageNumber, pageSize);
    }

    public async Task<IQueryable<Book>> GetBooksQueryAsync(BookFilterDto filter, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(filter, nameof(filter), _localizer.GetString(SharedResources.FilterRequired));

        var query = BuildBaseQuery();
        query = ApplyFilters(query, filter.SearchQuery, filter.TagIds, filter.CreatedByUserId);
        query = ApplySorting(query, filter.SortBy, filter.SortDescending);

        return query;
    }

    private IQueryable<Book> BuildBaseQuery()
    {
        return _context.Books
            .Include(b => b.Tags)
            .AsNoTracking();
    }

    private IQueryable<Book> ApplyFilters(
        IQueryable<Book> query,
        string? searchTerm,
        IEnumerable<Guid>? tagIds,
        Guid? createdByUserId)
    {
        if (!string.IsNullOrEmpty(searchTerm)) {
            var searchLower = searchTerm.ToLower();
            query = query.Where(b => b.Title.ToLower().Contains(searchLower) ||
                                    b.Author.ToLower().Contains(searchLower));
        }
        
        if (tagIds != null && tagIds.Any()) {
            query = query.Where(b => b.Tags.Any(t => tagIds.Contains(t.TagId)));
        }

        if (createdByUserId.HasValue) {
            query = query.Where(b => b.CreatedByUserId == createdByUserId.Value);
        }

        return query;
    }

    private IQueryable<Book> ApplySorting(IQueryable<Book> query, string? sortBy, bool sortDescending)
    {
        BookSortBy sortByEnum;
        try {
            sortByEnum = string.IsNullOrEmpty(sortBy) ? BookSortBy.CreatedAt : Enum.Parse<BookSortBy>(sortBy, true);
        } 
        catch {
            sortByEnum = BookSortBy.CreatedAt; 
        }

        return sortByEnum switch
        {
            BookSortBy.Title => sortDescending
                ? query.OrderByDescending(b => b.Title)
                : query.OrderBy(b => b.Title),
            BookSortBy.CreatedAt => sortDescending
                ? query.OrderByDescending(b => b.CreatedAt)
                : query.OrderBy(b => b.CreatedAt),
            _ => query.OrderBy(b => b.CreatedAt)
        };
    }

    private async Task<int> GetTotalCountAsync(IQueryable<Book> query, CancellationToken cancellationToken)
    {
        return await query.CountAsync(cancellationToken);
    }

    private async Task<List<Book>> GetPagedItemsAsync(
        IQueryable<Book> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private PagedResponseDto<BookDto> CreatePagedResult(
        List<Book> items,
        int totalItems,
        int pageNumber,
        int pageSize)
    {
        return new PagedResponseDto<BookDto>
        {
            Items = _mapper.Map<List<BookDto>>(items),
            TotalItems = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<BookDto> GetBookByIdAsync(Guid bookId, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(bookId, nameof(bookId), _localizer.GetString(SharedResources.BookIdRequired));

        var book = await _context.Books
            .Include(b => b.Tags)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == bookId, cancellationToken);
        Guard.AgainstNull(
            book,
            nameof(bookId),
            _localizer.GetString(SharedResources.BookNotFound),
            bookId.ToString());

        return _mapper.Map<BookDto>(book);
    }
}
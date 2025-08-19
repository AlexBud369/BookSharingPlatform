using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Application.DTOs.Book;
using Application.DTOs;

namespace Application.Interfaces;

public interface IBookQueryService
{
    Task<PagedResponseDto<BookDto>> GetBooksAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm,
        IEnumerable<Guid>? tagIds,
        Guid? createdByUserId,
        string? sortBy,
        bool sortDescending,
        CancellationToken cancellationToken);
    Task<BookDto> GetBookByIdAsync(Guid bookId, CancellationToken cancellationToken);
    Task<IQueryable<Book>> GetBooksQueryAsync(BookFilterDto filter, CancellationToken cancellationToken);

}

using Application.Common;
using Application.DTOs;
using Application.DTOs.Book;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Application.Features.Books.Queries;

public static class GetAllBooks
{
    public class Query : IRequest<PagedResponseDto<BookDto>>
    {
        public BookFilterDto Filter { get; set; } = new();
    }

    public class Handler : IRequestHandler<Query, PagedResponseDto<BookDto>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IBookQueryService _bookQueryService;

        public Handler(
            IMapper mapper,
            IStringLocalizer<SharedResources> localizer,
            IBookQueryService bookQueryService)
        {
            _mapper = mapper;
            _localizer = localizer;
            _bookQueryService = bookQueryService;
            Guard.Initialize(_localizer);
        }

        public async Task<PagedResponseDto<BookDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            ValidateRequest(request);
            var (books, totalItems) = await GetBooksAsync(request.Filter, cancellationToken);
            return MapToPagedResponse(books, request.Filter, totalItems);
        }

        private void ValidateRequest(Query request)
        {
            Guard.AgainstNull(request.Filter, nameof(request.Filter), _localizer.GetString(SharedResources.FilterRequired));
        }

        private async Task<(IEnumerable<Book> Books, int TotalItems)> GetBooksAsync(BookFilterDto filter, CancellationToken cancellationToken)
        {
            var booksQuery = await _bookQueryService.GetBooksQueryAsync(filter, cancellationToken);
            var totalItems = await booksQuery.CountAsync(cancellationToken);
            var books = await booksQuery
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);
            return (books, totalItems);
        }

        private PagedResponseDto<BookDto> MapToPagedResponse(IEnumerable<Book> books, BookFilterDto filter, int totalItems)
        {
            var bookDtos = _mapper.Map<IEnumerable<BookDto>>(books);
            Guard.AgainstNull(bookDtos, nameof(bookDtos), _localizer.GetString(SharedResources.BookNotFound));
            return new PagedResponseDto<BookDto>
            {
                Items = bookDtos,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalItems = totalItems
            };
        }
    }
}
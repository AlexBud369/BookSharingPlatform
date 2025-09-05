using Application.Common;
using Application.DTOs.Book;
using Application.Interfaces;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Persistence.Data;

namespace Application.Features.Books.Queries;

public static class GetBookById
{
    public class Query : IRequest<BookDto>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, BookDto>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public Handler(
            AppDbContext context,
            IMapper mapper,
            IStringLocalizer<SharedResources> localizer)
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
            Guard.Initialize(_localizer);
        }

        public async Task<BookDto> Handle(Query request, CancellationToken cancellationToken)
        {
            ValidateRequest(request);
            var book = await GetBookAsync(request.Id, cancellationToken);
            return MapToDto(book);
        }

        private void ValidateRequest(Query request)
        {
            Guard.AgainstEmptyGuid(request.Id, nameof(request.Id), _localizer.GetString(SharedResources.BookIdRequired));
        }

        private async Task<Book> GetBookAsync(Guid bookId, CancellationToken cancellationToken)
        {
            var book = await _context.Books
                .Include(b => b.Tags)
                .FirstOrDefaultAsync(b => b.Id == bookId, cancellationToken);
            Guard.AgainstNull(book, nameof(bookId), _localizer.GetString(SharedResources.BookNotFound));
            return book;
        }

        private BookDto MapToDto(Book book)
        {
            var bookDto = _mapper.Map<BookDto>(book);
            Guard.AgainstNull(bookDto, nameof(bookDto), _localizer.GetString(SharedResources.BookNotFound));
            return bookDto;
        }
    }
}
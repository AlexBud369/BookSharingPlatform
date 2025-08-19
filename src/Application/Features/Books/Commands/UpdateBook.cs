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

namespace Application.Features.Books.Commands;

public static class UpdateBook
{
    public class Command : IRequest<BookDto>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public BookUpdateDto Book { get; set; } = new();
    }

    public class Handler : IRequestHandler<Command, BookDto>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IBookAccessService _bookAccessService;
        private readonly ITagService _tagService;

        public Handler(
            AppDbContext context,
            IMapper mapper,
            IStringLocalizer<SharedResources> localizer,
            IBookAccessService bookAccessService,
            ITagService tagService)
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
            _bookAccessService = bookAccessService;
            _tagService = tagService;
            Guard.Initialize(_localizer);
        }

        public async Task<BookDto> Handle(Command request, CancellationToken cancellationToken)
        {
            ValidateRequest(request);
            var book = await GetBookAsync(request.Id, cancellationToken);

            await ValidateAccessAsync(book, request.UserId, cancellationToken);
            UpdateBook(book, request.Book);

            await UpdateTagsAsync(book, request.Book.Tags, cancellationToken);
            await SaveChangesAsync(cancellationToken);
            
            return MapToDto(book);
        }

        private void ValidateRequest(Command request)
        {
            Guard.AgainstEmptyGuid(request.Id, nameof(request.Id), _localizer.GetString(SharedResources.BookIdRequired));
            Guard.AgainstEmptyGuid(request.UserId, nameof(request.UserId), _localizer.GetString(SharedResources.UserIdRequired));
            Guard.AgainstNull(request.Book, nameof(request.Book), _localizer.GetString(SharedResources.BookDataRequired));
        }

        private async Task<Book> GetBookAsync(Guid bookId, CancellationToken cancellationToken)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId, cancellationToken);
            Guard.AgainstNull(book, nameof(bookId), _localizer.GetString(SharedResources.BookNotFound));
            return book;
        }

        private async Task ValidateAccessAsync(Book book, Guid userId, CancellationToken cancellationToken)
        {
            var hasAccess = await _bookAccessService.CanEditBookAsync(userId, book.Id, cancellationToken);
            Guard.AgainstFalse(hasAccess, nameof(userId), _localizer.GetString(SharedResources.UnauthorizedAccess));
        }

        private void UpdateBook(Book book, BookUpdateDto bookDto)
        {
            if (bookDto.Title != null) {
                book.Title = bookDto.Title;
            }
            if (bookDto.Author != null) {
                book.Author = bookDto.Author;
            }
            if (bookDto.Description != null) { 
                book.Description = bookDto.Description;
            }
            if (bookDto.CoverImageUrl != null) {
                book.CoverImageUrl = bookDto.CoverImageUrl;
            }
        }

        private async Task UpdateTagsAsync(Book book, IEnumerable<Guid>? tagIds, CancellationToken cancellationToken)
        {
            if (tagIds != null) {
                var tagNames = await _tagService.GetTagNamesByIdsAsync(tagIds, cancellationToken);
                Guard.AgainstNull(tagNames, nameof(tagNames), _localizer.GetString(SharedResources.TagNotFound));
                await _tagService.UpdateTagsForBookAsync(book, tagNames, cancellationToken);
            }
        }

        private async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        private BookDto MapToDto(Book book)
        {
            var bookDto = _mapper.Map<BookDto>(book);
            Guard.AgainstNull(bookDto, nameof(bookDto), _localizer.GetString(SharedResources.BookNotFound));
            
            return bookDto;
        }
    }
}
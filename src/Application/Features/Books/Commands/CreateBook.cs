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

public static class CreateBook
{
    public class Command : IRequest<BookDto>
    {
        public BookCreateDto Book { get; set; } = new();
        public Guid UserId { get; set; }
    }

    public class Handler : IRequestHandler<Command, BookDto>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ITagService _tagService;

        public Handler(
            AppDbContext context,
            IMapper mapper,
            IStringLocalizer<SharedResources> localizer,
            ITagService tagService)
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
            _tagService = tagService;
            Guard.Initialize(_localizer);
        }

        public async Task<BookDto> Handle(Command request, CancellationToken cancellationToken)
        {
            ValidateRequest(request);
            var user = await ValidateUserAsync(request.UserId, cancellationToken);
            var book = CreateBook(request.Book, request.UserId);
            await AddTagsAsync(book, request.Book.Tags, cancellationToken);
            await SaveBookAsync(book, cancellationToken);

            return _mapper.Map<BookDto>(book);
        }

        private void ValidateRequest(Command request)
        {
            Guard.AgainstNull(request.Book, nameof(request.Book), _localizer.GetString(SharedResources.BookDataRequired));
        }

        private async Task<ApplicationUser> ValidateUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            Guard.AgainstEmptyGuid(userId, nameof(userId), _localizer.GetString(SharedResources.UserIdRequired));
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            Guard.AgainstNull(user, nameof(userId), _localizer.GetString(SharedResources.UserNotFound));

            return user;
        }

        private Book CreateBook(BookCreateDto bookDto, Guid userId)
        {
            var book = _mapper.Map<Book>(bookDto);
            book.CreatedByUserId = userId;

            return book;
        }

        private async Task AddTagsAsync(Book book, IEnumerable<Guid>? tagIds, CancellationToken cancellationToken)
        {
            if (tagIds == null || !tagIds.Any()) {
                return;
            }

            var tagNames = await _tagService.GetTagNamesByIdsAsync(tagIds, cancellationToken);
            Guard.AgainstNull(tagNames, nameof(tagNames), _localizer.GetString(SharedResources.TagNotFound));
            Guard.AgainstFalse(tagNames.Any(), nameof(tagNames), _localizer.GetString(SharedResources.TagNotFound));
            await _tagService.AddTagsToBookAsync(book, tagNames, cancellationToken);
        }

        private async Task SaveBookAsync(Book book, CancellationToken cancellationToken)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
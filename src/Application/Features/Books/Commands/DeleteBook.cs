using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Persistence.Data;

namespace Application.Features.Books.Commands;

public static class DeleteBook
{
    public class Command : IRequest
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly AppDbContext _context;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IBookAccessService _bookAccessService;

        public Handler(
            AppDbContext context,
            IStringLocalizer<SharedResources> localizer,
            IBookAccessService bookAccessService)
        {
            _context = context;
            _localizer = localizer;
            _bookAccessService = bookAccessService;
            Guard.Initialize(_localizer);
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            ValidateRequest(request);
            var book = await GetBookAsync(request.Id, cancellationToken);
            var canDelete = await _bookAccessService.CanDeleteBookAsync(request.UserId, book.Id, cancellationToken);
            Guard.AgainstFalse(canDelete, nameof(request.UserId), _localizer.GetString(SharedResources.UnauthorizedAccess));
            await DeleteBookAsync(book, cancellationToken);
        }

        private void ValidateRequest(Command request)
        {
            Guard.AgainstEmptyGuid(request.Id, nameof(request.Id), _localizer.GetString(SharedResources.BookIdRequired));
            Guard.AgainstEmptyGuid(request.UserId, nameof(request.UserId), _localizer.GetString(SharedResources.UserIdRequired));
        }

        private async Task<Book> GetBookAsync(Guid bookId, CancellationToken cancellationToken)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId, cancellationToken);
            Guard.AgainstNull(book, nameof(bookId), _localizer.GetString(SharedResources.BookNotFound));

            return book;
        }

        private async Task DeleteBookAsync(Book book, CancellationToken cancellationToken)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
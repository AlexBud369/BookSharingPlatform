using Application.Common;
using Application.DTOs.Book;
using Application.Interfaces;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Persistence.Data;

namespace Application.Features.Books.Commands;

public static class UploadBookCover
{
    public class Command : IRequest<BookDto>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public IFormFile CoverImage { get; set; } = null!;
    }

    public class Handler : IRequestHandler<Command, BookDto>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IBookAccessService _bookAccessService;
        private readonly IImageService _imageService;

        public Handler(
            AppDbContext context,
            IMapper mapper,
            IStringLocalizer<SharedResources> localizer,
            IBookAccessService bookAccessService,
            IImageService imageService)
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
            _bookAccessService = bookAccessService;
            _imageService = imageService;
            Guard.Initialize(_localizer);
        }

        public async Task<BookDto> Handle(Command request, CancellationToken cancellationToken)
        {
            ValidateRequest(request);
            var book = await GetBookAsync(request.Id, cancellationToken);

            await ValidateAccessAsync(book, request.UserId, cancellationToken);
            var imageUrl = await UploadCoverImageAsync(request.CoverImage, cancellationToken);
            book.CoverImageUrl = imageUrl;
            await SaveChangesAsync(cancellationToken);

            return MapToDto(book);
        }

        private void ValidateRequest(Command request)
        {
            Guard.AgainstEmptyGuid(request.Id, nameof(request.Id), _localizer.GetString(SharedResources.BookIdRequired));
            Guard.AgainstEmptyGuid(request.UserId, nameof(request.UserId), _localizer.GetString(SharedResources.UserIdRequired));
            Guard.AgainstNull(request.CoverImage, nameof(request.CoverImage), _localizer.GetString(SharedResources.CoverImageRequired));
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

        private async Task<string> UploadCoverImageAsync(IFormFile coverImage, CancellationToken cancellationToken)
        {
            var imageUrl = await _imageService.UploadImageAsync(coverImage, cancellationToken);
            Guard.AgainstNull(imageUrl, nameof(imageUrl), _localizer.GetString(SharedResources.CoverImageUploadFailed));
            
            return imageUrl;
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
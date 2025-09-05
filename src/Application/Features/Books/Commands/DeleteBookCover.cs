using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Persistence.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Books.Commands;

public static class DeleteBookCover
{
    public class Command : IRequest
    {
        public string FileName { get; set; }
        public Guid BookId { get; set; }
        public Guid UserId { get; set; }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IBookAccessService _bookAccessService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public Handler(
            IFileStorageService fileStorageService,
            IBookAccessService bookAccessService,
            UserManager<ApplicationUser> userManager,
            AppDbContext context,
            IStringLocalizer<SharedResources> localizer)
        {
            _fileStorageService = fileStorageService;
            _bookAccessService = bookAccessService;
            _userManager = userManager;
            _context = context;
            _localizer = localizer;
            Guard.Initialize(_localizer);
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            Guard.AgainstEmptyString(request.FileName, nameof(request.FileName), _localizer.GetString(SharedResources.FileNameRequired));
            Guard.AgainstEmptyGuid(request.BookId, nameof(request.BookId), _localizer.GetString(SharedResources.BookIdRequired));

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            Guard.AgainstNull(user, nameof(request.UserId), _localizer.GetString(SharedResources.UserNotFound), request.UserId.ToString());

            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == request.BookId, cancellationToken);
            Guard.AgainstNull(book, nameof(request.BookId), _localizer.GetString(SharedResources.BookNotFound), request.BookId.ToString());

            await _bookAccessService.ValidateBookAccessAsync(book, user.Id, false, cancellationToken);

            await _fileStorageService.DeleteFileAsync(request.FileName, cancellationToken);
            book.CoverImageUrl = null;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
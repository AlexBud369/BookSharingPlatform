using Application.Common;
using Application.DTOs.Book;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Persistence.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Books.Commands;

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, BookDto>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly ITagService _tagService;
    private readonly IBookAccessService _bookAccessService;

    public UpdateBookCommandHandler(
        AppDbContext context,
        IMapper mapper,
        IStringLocalizer<SharedResources> localizer,
        ITagService tagService,
        IBookAccessService bookAccessService)
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
        _tagService = tagService;
        _bookAccessService = bookAccessService;
        Guard.Initialize(_localizer);
    }

    public async Task<BookDto> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(request.Id, nameof(request.Id), _localizer.GetString(SharedResources.BookIdRequired));
        Guard.AgainstEmptyGuid(request.UserId, nameof(request.UserId), _localizer.GetString(SharedResources.UserIdRequired));
        Guard.AgainstNull(request.Book, nameof(request.Book), _localizer.GetString(SharedResources.BookDataRequired));

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        Guard.AgainstNull(user, nameof(request.UserId), _localizer.GetString(SharedResources.UserNotFound), request.UserId.ToString());

        var book = await _context.Books
            .Include(b => b.Tags)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        Guard.AgainstNull(book, nameof(request.Id), _localizer.GetString(SharedResources.BookNotFound), request.Id.ToString());

        await _bookAccessService.ValidateBookAccessAsync(book, request.UserId, false, cancellationToken);

        _mapper.Map(request.Book, book);

        var bookTagEntries = await _context.Set<Dictionary<string, object>>("BookTag")
            .Where(bt => (Guid)bt["BookId"] == book.Id)
            .ToListAsync(cancellationToken);
        _context.RemoveRange(bookTagEntries);

        if (request.Book.Tags != null) {
            var tagNames = await _tagService.GetTagNamesByIdsAsync(request.Book.Tags, cancellationToken);
            await _tagService.AddTagsToBookAsync(book, tagNames, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<BookDto>(book);
    }
}
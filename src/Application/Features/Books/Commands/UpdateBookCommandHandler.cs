using Application.Common;
using Application.DTOs.Book;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
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
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly ITagService _tagService;
    private readonly IBookAccessService _bookService;

    public UpdateBookCommandHandler(
        AppDbContext context,
        IMapper mapper,
        IStringLocalizer<SharedResource> localizer,
        ITagService tagService,
        IBookAccessService bookService)
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
        _tagService = tagService;
        _bookService = bookService;
        Guard.Initialize(_localizer);
    }

    public async Task<BookDto> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(request.Book, nameof(request.Book), "BookDataRequired");
        Guard.AgainstEmptyString(request.Book.Title, nameof(request.Book.Title), "EmptyString", nameof(request.Book.Title));

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        Guard.AgainstNull(user, nameof(request.UserId), "UserNotFound", request.UserId.ToString());

        var book = await _context.Books
            .Include(b => b.Tags)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        Guard.AgainstNull(book, nameof(request.Id), "BookNotFound", request.Id.ToString());

        await _bookAccessService.ValidateBookAccessAsync(book, request.UserId, false, cancellationToken);

        _mapper.Map(request.Book, book);
        book.TagsList.Clear();
        await _tagService.AddTagsToBookAsync(book, request.Book.Tags, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<BookDto>(book);
    }
}

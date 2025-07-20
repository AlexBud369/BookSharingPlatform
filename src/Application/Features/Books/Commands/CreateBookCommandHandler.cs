using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.DTOs.Book;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Application.Features.Books.Commands;

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly ITagService _tagService;

    public CreateBookCommandHandler(
        AppDbContext context,
        IMapper mapper,
         IStringLocalizer<SharedResource> localizer,
        ITagService tagService)
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
        _tagService = tagService;
        Guard.Initialize(_localizer);
    }

    public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(request.Book, nameof(request.Book), "BookDataRequired");
        Guard.AgainstEmptyString(request.Book.Title, nameof(request.Book.Title), "EmptyString", nameof(request.Book.Title));

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        Guard.AgainstNull(user, nameof(request.UserId), "UserNotFound", request.UserId.ToString());

        var book = _mapper.Map<Book>(request.Book);
        book.CreatedByUserId = request.UserId;

        await _tagService.AddTagsToBookAsync(book, request.Book.Tags, cancellationToken);

        _context.Books.Add(book);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<BookDto>(book);
    }

}

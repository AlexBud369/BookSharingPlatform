using Application.DTOs.Book;
using Application.Common.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Application.Features.Books.Commands;

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CreateBookCommandHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user == null)
        { 
            throw new UserNotFoundException(request.UserId);
        }

        var book = _mapper.Map<Book>(request.Book);
        book.CreatedByUserId = request.UserId;

        if (request.Book.Tags != null)
        {
            var tagNames = request.Book.Tags.ToList();

            var existingTags = await _context.Tags
                .Where(t => tagNames.Contains(t.tagName))
                .ToListAsync(cancellationToken);
            book.TagsList.AddRange(existingTags);

            var newTagNames = tagNames.Except(existingTags.Select(t => t.tagName)).ToList();

            book.TagsList.AddRange(newTagNames.Select(name => new Tag { tagName = name }));
        }

        _context.Books.Add(book);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<BookDto>(book);
    }

}

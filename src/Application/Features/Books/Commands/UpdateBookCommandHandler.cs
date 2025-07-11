using Application.DTOs.Book;
using Application.Common.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Books.Commands;

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, BookDto>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public UpdateBookCommandHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BookDto> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user == null)
        { 
            throw new UserNotFoundException(request.UserId);
        }

        var book = await _context.Books
            .Include(b => b.Tags)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        if (book == null)
        {
            throw new BookNotFoundException(request.Id);
        }
        if (book.CreatedByUserId != request.UserId)
        {
            throw new UnauthorizedAccessException("Only the book owner can update it");
        }

        _mapper.Map(request.Book, book);
        if (request.Book.Tags != null)
        {
            book.TagsList.Clear();
            var tagNames = request.Book.Tags.ToList();
            var existingTags = await _context.Tags
                .Where(t => tagNames.Contains(t.tagName))
                .ToListAsync(cancellationToken);
            book.TagsList.AddRange(existingTags);
            var newTagNames = tagNames.Except(existingTags.Select(t => t.tagName)).ToList();
            book.TagsList.AddRange(newTagNames.Select(name => new Tag { tagName = name }));
        }
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<BookDto>(book);
    }
}

using Application.DTOs.Book;
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

    public async Task<BookDto> Handler(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var book = _mapper.Map<Book>(request.Book);
        book.CreatedByUser = request.UserId;

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

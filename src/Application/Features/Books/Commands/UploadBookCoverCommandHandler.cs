using Application.Common.Exceptions;
using Application.DTOs.Book;
using AutoMapper;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Books.Commands;

public class UploadBookCoverCommandHandler : IRequestHandler<UploadBookCoverCommand, BookDto>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public UploadBookCoverCommandHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BookDto> Handle(UploadBookCoverCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user == null)
        {
            throw new UserNotFoundException(request.UserId);
        }

        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        if (book == null)
        {
            throw new BookNotFoundException(request.Id);
        }
        if (book.CreatedByUserId != request.UserId)
        {
            throw new UnauthorizedAccessException("Only the book owner can upload a cover image");
        }

        book.CoverImageUrl = request.CoverImageUrl;
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<BookDto>(book);
    }
}
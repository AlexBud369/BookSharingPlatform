using Application.Common.Exceptions;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Books.Commands;

public class DeleteBookCommandHandler
{
    private readonly AppDbContext _context;

    public DeleteBookCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteBookCommand request, CancellationToken cancellationToken)
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
            throw new UnauthorizedAccessException("Only the book owner can delete it");
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Application.Common.Exceptions;
using Domain.Entities;
using Infrastructure.Data;
namespace Application.Features.Books.Commands;

public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand>
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteBookCommandHandler(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
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

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        if (book.CreatedByUserId != request.UserId && !isAdmin)
        {
            throw new UnauthorizedAccessException("Only the book owner or an admin can delete it");
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

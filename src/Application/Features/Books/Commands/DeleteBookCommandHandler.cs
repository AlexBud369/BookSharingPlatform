using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Application.Features.Books.Commands;

public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand>
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly IBookAccessService _bookService;

    public DeleteBookCommandHandler(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        IStringLocalizer<SharedResource> localizer,
        IBookAccessService bookService)
    {
        _context = context;
        _userManager = userManager;
        _localizer = localizer;
        _bookService = bookService;
        Guard.Initialize(_localizer);
    }

    public async Task Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        Guard.AgainstNull(user, nameof(request.UserId), "UserNotFound", request.UserId.ToString());

        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        Guard.AgainstNull(book, nameof(request.Id), "BookNotFound", request.Id.ToString());

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        await _bookAccessService.ValidateBookAccessAsync(book, request.UserId, isAdmin, cancellationToken);

        _context.Books.Remove(book);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

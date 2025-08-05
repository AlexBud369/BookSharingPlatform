using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Books.Commands;

public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, Unit>
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly IBookAccessService _bookService;

    public DeleteBookCommandHandler(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        IStringLocalizer<SharedResources> localizer,
        IBookAccessService bookService)
    {
        _context = context;
        _userManager = userManager;
        _localizer = localizer;
        _bookService = bookService;
        Guard.Initialize(_localizer);
    }

    public async Task<Unit> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(request.Id, nameof(request.Id), _localizer.GetString(SharedResources.BookIdRequired));
        Guard.AgainstEmptyGuid(request.UserId, nameof(request.UserId), _localizer.GetString(SharedResources.UserIdRequired));

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        Guard.AgainstNull(user, nameof(request.UserId), _localizer.GetString(SharedResources.UserNotFound), request.UserId.ToString());

        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        Guard.AgainstNull(book, nameof(request.Id), _localizer.GetString(SharedResources.BookNotFound), request.Id.ToString());

        var isAdmin = await _userManager.IsInRoleAsync(user, UserRole.Admin.ToString());
        await _bookService.ValidateBookAccessAsync(book, request.UserId, isAdmin, cancellationToken);

        _context.Books.Remove(book);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
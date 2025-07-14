using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Commands;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var admin = await _userManager.FindByIdAsync(request.AdminId.ToString());
        if (admin == null)
        {
            throw new UserNotFoundException(request.AdminId);
        }

        if (!await _userManager.IsInRoleAsync(admin, "Admin"))
        {
            throw new AdminOnlyAccessException("Only admins can delete users");
        }

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            throw new UserNotFoundException(request.UserId);
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            throw new Exception($"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}
using MediatR;
using Microsoft.AspNetCore.Identity;
using Application.Common.Exceptions;
using Domain.Entities;

namespace Application.Features.Users.Commands;

public class BlockUserCommandHandler : IRequestHandler<BlockUserCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public BlockUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        var admin = await _userManager.FindByIdAsync(request.AdminId.ToString());
        if (admin == null)
        {
            throw new UserNotFoundException(request.AdminId);
        }

        if (!await _userManager.IsInRoleAsync(admin, "Admin")) 
        {
            throw new AdminOnlyAccessException("Only admins can block users");
        }

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            throw new UserNotFoundException(request.UserId);
        }

        user.IsBlocked = true;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException("Failed to block user");
        }
    }
}
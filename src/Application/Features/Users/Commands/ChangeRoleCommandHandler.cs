using MediatR;
using Microsoft.AspNetCore.Identity;
using Application.Common.Exceptions;
using Domain.Entities;

namespace Application.Features.Users.Commands;

public class ChangeRoleCommandHandler : IRequestHandler<ChangeRoleCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public ChangeRoleCommandHandler(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }


    public async Task Handle(ChangeRoleCommand request, CancellationToken cancellationToken)
    {
        var admin = await _userManager.FindByIdAsync(request.AdminId.ToString());
        if (admin == null)
        {
            throw new UserNotFoundException(request.AdminId);
        }
        if (!await _userManager.IsInRoleAsync(admin, "Admin"))
        {
            throw new AdminOnlyAccessException("Only admins can change user roles");
        }

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            throw new UserNotFoundException(request.UserId);
        }

        if (!await _roleManager.RoleExistsAsync(request.Role))
        {
            var role = new ApplicationRole { Name = request.Role };
            var roleResult = await _roleManager.CreateAsync(role);
            if (!roleResult.Succeeded)
            {
                throw new UnauthorizedAccessException($"Failed to create role: {request.Role}");
            }
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        var result = await _userManager.AddToRoleAsync(user, request.Role);
        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException($"Failed to assign role: {request.Role}");
        }
    }
}

using Application.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands;

public class ChangeRoleCommandHandler : IRequestHandler<ChangeRoleCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public ChangeRoleCommandHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IStringLocalizer<SharedResources> localizer)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    public async Task<Unit> Handle(ChangeRoleCommand request, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(request.UserId, nameof(request.UserId), _localizer.GetString(SharedResources.UserIdRequired));
        Guard.AgainstEmptyGuid(request.AdminId, nameof(request.AdminId), _localizer.GetString(SharedResources.AdminIdRequired));
        Guard.AgainstEmptyString(request.Role, nameof(request.Role), _localizer.GetString(SharedResources.RoleRequired));

        var admin = await _userManager.FindByIdAsync(request.AdminId.ToString());
        Guard.AgainstNull(admin, nameof(request.AdminId), _localizer.GetString(SharedResources.UserNotFound), request.AdminId.ToString());

        var isAdmin = await _userManager.IsInRoleAsync(admin, UserRole.Admin.ToString());
        Guard.AgainstFalse(!isAdmin, nameof(request.AdminId), _localizer.GetString(SharedResources.AdminOnlyAccess));

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        Guard.AgainstNull(user, nameof(request.UserId), _localizer.GetString(SharedResources.UserNotFound), request.UserId.ToString());

        if (!await _roleManager.RoleExistsAsync(request.Role)) {
            var role = new ApplicationRole { Name = request.Role };
            var roleResult = await _roleManager.CreateAsync(role);
            Guard.AgainstFalse(!roleResult.Succeeded, nameof(request.Role), _localizer.GetString(SharedResources.FailedToCreateRole), request.Role);
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        var result = await _userManager.AddToRoleAsync(user, request.Role);
        Guard.AgainstFalse(!result.Succeeded, nameof(request.Role), _localizer.GetString(SharedResources.FailedToAssignRole), request.Role);
        
        return Unit.Value;
    }
}
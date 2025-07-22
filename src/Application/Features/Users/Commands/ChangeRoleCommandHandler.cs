using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Application.Features.Users.Commands;

public class ChangeRoleCommandHandler : IRequestHandler<ChangeRoleCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public ChangeRoleCommandHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IStringLocalizer<SharedResource> localizer)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }


    public async Task Handle(ChangeRoleCommand request, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyString(request.Role, nameof(request.Role), "RoleRequired");

        var admin = await _userManager.FindByIdAsync(request.AdminId.ToString());
        Guard.AgainstNull(admin, nameof(request.AdminId), "UserNotFound", request.AdminId.ToString());

        var isAdmin = await _userManager.IsInRoleAsync(admin, "Admin");
        Guard.Against(!isAdmin, nameof(request.AdminId), "AdminOnlyAccess");

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        Guard.AgainstNull(user, nameof(request.UserId), "UserNotFound", request.UserId.ToString());

        if (!await _roleManager.RoleExistsAsync(request.Role)) {
            var role = new ApplicationRole { Name = request.Role };
            var roleResult = await _roleManager.CreateAsync(role);
            Guard.Against(!roleResult.Succeeded, nameof(request.Role), "FailedToCreateRole", request.Role);
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        var result = await _userManager.AddToRoleAsync(user, request.Role);
        Guard.Against(!result.Succeeded, nameof(request.Role), "FailedToAssignRole", request.Role);
    }
}

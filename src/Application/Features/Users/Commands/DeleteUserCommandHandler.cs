using Application.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public DeleteUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        IStringLocalizer<SharedResources> localizer)
    {
        Guard.AgainstNull(userManager, nameof(userManager), localizer.GetString(SharedResources.UserManagerRequired));
        Guard.AgainstNull(localizer, nameof(localizer), localizer.GetString(SharedResources.LocalizerRequired));

        _userManager = userManager;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(request.UserId, nameof(request.UserId), _localizer.GetString(SharedResources.UserIdRequired));
        Guard.AgainstEmptyGuid(request.AdminId, nameof(request.AdminId), _localizer.GetString(SharedResources.AdminIdRequired));

        var admin = await _userManager.FindByIdAsync(request.AdminId.ToString());
        Guard.AgainstNull(admin, nameof(request.AdminId), _localizer.GetString(SharedResources.UserNotFound), request.AdminId.ToString());

        var isAdmin = await _userManager.IsInRoleAsync(admin, UserRole.Admin.ToString());
        Guard.Against(!isAdmin, nameof(request.AdminId), _localizer.GetString(SharedResources.AdminOnlyAccess));

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        Guard.AgainstNull(user, nameof(request.UserId), _localizer.GetString(SharedResources.UserNotFound), request.UserId.ToString());

        var result = await _userManager.DeleteAsync(user);
        Guard.Against(!result.Succeeded, nameof(user), _localizer.GetString(SharedResources.FailedToDeleteUser), string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}
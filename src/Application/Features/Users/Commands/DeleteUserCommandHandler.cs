using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Application.Features.Users.Commands;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public DeleteUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        IStringLocalizer<SharedResource> localizer)
    {
        _userManager = userManager;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var admin = await _userManager.FindByIdAsync(request.AdminId.ToString());
        Guard.AgainstNull(admin, nameof(request.AdminId), "UserNotFound", request.AdminId.ToString());

        var isAdmin = await _userManager.IsInRoleAsync(admin, "Admin");
        Guard.Against(!isAdmin, nameof(request.AdminId), "AdminOnlyAccess");

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        Guard.AgainstNull(user, nameof(request.UserId), "UserNotFound", request.UserId.ToString());

        var result = await _userManager.DeleteAsync(user);
        Guard.Against(!result.Succeeded, nameof(user), "FailedToDeleteUser", string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}
using Application.Common;
using Application.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands;

public class ChangeRole : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid AdminId { get; set; }
    public string Role { get; set; } = string.Empty;

    public class Handler
    {
        private readonly IUserService _userService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public Handler(
            IUserService userService,
            IStringLocalizer<SharedResources> localizer)
        {
            _userService = userService;
            _localizer = localizer;
            Guard.Initialize(_localizer);
        }

        public async Task<Unit> Handle(ChangeRole request, CancellationToken cancellationToken)
        {
            Guard.AgainstEmptyGuid(request.UserId, nameof(request.UserId), _localizer.GetString(SharedResources.UserIdRequired));
            Guard.AgainstEmptyGuid(request.AdminId, nameof(request.AdminId), _localizer.GetString(SharedResources.AdminIdRequired));
            Guard.AgainstEmptyString(request.Role, nameof(request.Role), _localizer.GetString(SharedResources.RoleRequired));

            await _userService.EnsureIsAdminAsync(request.AdminId, cancellationToken);
            await _userService.ChangeUserRoleAsync(request.UserId, request.Role, cancellationToken);

            return Unit.Value;
        }
    }
}
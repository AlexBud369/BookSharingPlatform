using Application.Common;
using Application.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands;

public class DeleteUser : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid AdminId { get; set; }

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

        public async Task<Unit> Handle(DeleteUser request, CancellationToken cancellationToken)
        {
            Guard.AgainstEmptyGuid(request.UserId, nameof(request.UserId), _localizer.GetString(SharedResources.UserIdRequired));
            Guard.AgainstEmptyGuid(request.AdminId, nameof(request.AdminId), _localizer.GetString(SharedResources.AdminIdRequired));

            await _userService.EnsureIsAdminAsync(request.AdminId, cancellationToken);
            await _userService.DeleteUserAsync(request.UserId, cancellationToken);

            return Unit.Value;
        }
    }
}
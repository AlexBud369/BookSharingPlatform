using Application.Common;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands;

public static class BlockUser
{
    public class Command : IRequest
    {
        public Guid UserId { get; set; }
        public Guid AdminId { get; set; }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly IUserService _userService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public Handler(IUserService userService, IStringLocalizer<SharedResources> localizer)
        {
            _userService = userService;
            _localizer = localizer;
            Guard.Initialize(_localizer);
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _userService.BlockUserAsync(request.UserId, request.AdminId, cancellationToken);
        }
    }
}
using Application.Common;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Application.Features.Auth.Commands;

public class Logout
{
    public class Command : IRequest
    {
        public Guid UserId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly IAuthService _authService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public Handler(
            IAuthService authService,
            IStringLocalizer<SharedResources> localizer)
        {
            _authService = authService;
            _localizer = localizer;
            Guard.Initialize(_localizer);
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            Guard.AgainstEmptyString(request.RefreshToken, nameof(request.RefreshToken), _localizer.GetString(SharedResources.RefreshTokenRequired));
            Guard.AgainstEmptyGuid(request.UserId, nameof(request.UserId), _localizer.GetString(SharedResources.UserIdRequired));

            await _authService.RevokeRefreshTokenAsync(request.RefreshToken, request.UserId, cancellationToken);

        }
    }
}


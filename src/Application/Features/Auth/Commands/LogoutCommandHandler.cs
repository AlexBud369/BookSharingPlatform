using Application.Common;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IAuthService _authService;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public LogoutCommandHandler(
        IAuthService authService,
        IStringLocalizer<SharedResources> localizer)
    {
        _authService = authService;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyString(request.RefreshToken, nameof(request.RefreshToken), _localizer.GetString(SharedResources.RefreshTokenRequired));
        Guard.AgainstEmptyGuid(request.UserId, nameof(request.UserId), _localizer.GetString(SharedResources.UserIdRequired));

        await _authService.RevokeRefreshTokenAsync(request.RefreshToken, request.UserId, cancellationToken);

        return Unit.Value;
    }
}
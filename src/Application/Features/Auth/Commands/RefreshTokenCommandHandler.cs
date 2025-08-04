using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public RefreshTokenCommandHandler(
        IAuthService authService,
        IStringLocalizer<SharedResources> localizer)
    {
        _authService = authService;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyString(request.RefreshToken, nameof(request.RefreshToken), _localizer.GetString(SharedResources.RefreshTokenRequired));

        var refreshTokenDto = new RefreshTokenDto { Token = request.RefreshToken };
        return await _authService.RefreshTokenAsync(refreshTokenDto, cancellationToken);
    }
}
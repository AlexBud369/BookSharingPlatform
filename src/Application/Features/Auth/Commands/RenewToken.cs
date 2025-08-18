using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Application.Features.Auth.Commands;

public static class RenewToken
{
    public class Command : IRequest<AuthResponseDto>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class Handler : IRequestHandler<Command, AuthResponseDto>
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

        public async Task<AuthResponseDto> Handle(Command request, CancellationToken cancellationToken)
        {
            Guard.AgainstEmptyString(request.RefreshToken, nameof(request.RefreshToken), _localizer.GetString(SharedResources.RefreshTokenRequired));

            var refreshTokenDto = new RefreshTokenDto { Token = request.RefreshToken };
            var result = await _authService.RefreshTokenAsync(refreshTokenDto, cancellationToken);
            Guard.AgainstNull(result, nameof(result), _localizer.GetString(SharedResources.RefreshTokenInvalid));
            return result;
        }
    }
}
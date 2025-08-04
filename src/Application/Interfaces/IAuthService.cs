using Application.DTOs;
using Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<string> GenerateJwtTokenAsync(ApplicationUser user, CancellationToken cancellationToken);
    Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken);
    Task RevokeRefreshTokenAsync(string refreshToken, Guid userId, CancellationToken cancellationToken);
    Task CreateUserAsync(ApplicationUser user, string password, string role, CancellationToken cancellationToken);
}

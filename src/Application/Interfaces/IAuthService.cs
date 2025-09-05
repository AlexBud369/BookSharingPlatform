using Domain.Entities;
using Domain.Enums;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<string> GenerateJwtTokenAsync(ApplicationUser user, CancellationToken cancellationToken);
    Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken);
    Task RevokeRefreshTokenAsync(string refreshToken, Guid userId, CancellationToken cancellationToken);
    Task CreateUserAsync(ApplicationUser user, string password, UserRole role, CancellationToken cancellationToken);
}
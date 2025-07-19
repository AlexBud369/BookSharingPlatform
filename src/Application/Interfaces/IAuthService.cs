using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<string> GenerateJwtTokenAsync(ApplicationUser user, CancellationToken cancellationToken);
    Task<RefreshToken> GenerateRefreshTokenAsync(string userId, CancellationToken cancellationToken);
    Task CreateUserAsync(ApplicationUser user, string password, string role, CancellationToken cancellationToken);
}

using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces;

public interface IJwtTokenService
{
    Task<string> GenerateJwtTokenAsync(ApplicationUser user, CancellationToken cancellationToken);
}
using Domain.Entities;

namespace Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> GetByTokenAsync(string token);
    Task AddAsync(RefreshToken token);
    Task UpdateAsync(RefreshToken token);
    Task DeleteAsync(Guid tokenId);
}
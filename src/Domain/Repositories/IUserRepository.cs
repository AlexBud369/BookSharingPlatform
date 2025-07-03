using Domain.Entities;

namespace Domain.Repositories;

public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid userId);
    Task<User> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid userId);
    Task BlockAsync(Guid userId);
    Task UnblockAsync(Guid userId);
}
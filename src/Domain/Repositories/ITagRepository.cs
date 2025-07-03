using Domain.Entities;

namespace Domain.Repositories;

public interface ITagRepository
{
    Task<Tag> GetByIdAsync(Guid tagId);
    Task<IEnumerable<Tag>> GetAllAsync();
    Task AddAsync(Tag tag);
    Task UpdateAsync(Tag tag);
    Task DeleteAsync(Guid tagId);
}
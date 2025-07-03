using Domain.Entities;

namespace Domain.Repositories;

public interface IBookRepository
{
    Task<Book> GetByIdAsync(Guid bookId);
    Task<IEnumerable<Book>> GetAllAsync(string? search, IEnumerable<string>? tags, Guid? createdByUserId, int page, int pageSize);
    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(Guid bookId);
}


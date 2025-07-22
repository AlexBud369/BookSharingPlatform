using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces;

public interface IImageService
{
    Task<string> UpdateBookCoverAsync(
        Book book,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken);
}
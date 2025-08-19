using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IImageService
{
    Task<string> UpdateBookCoverAsync(
        Book book,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken,
         bool deleteOldCover = true);
    Task DeleteBookCoverAsync(Book book, CancellationToken cancellationToken);
    Task<string> UploadImageAsync(IFormFile file, CancellationToken cancellationToken);
}
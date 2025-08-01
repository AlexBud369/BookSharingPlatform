namespace Application.Interfaces;

public interface ISignedUrlService
{
    Task<string> GenerateSignedUrlAsync(string fileName, int expiresInSeconds, CancellationToken cancellationToken = default);
}

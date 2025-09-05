using Amazon.S3;
using Amazon.S3.Model;
using Application.Common;
using Application.Interfaces;
using Infrastructure.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class SignedUrlService : ISignedUrlService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly ILogger<SignedUrlService> _logger;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public SignedUrlService(
        IOptions<SupabaseOptions> options,
        ILogger<SignedUrlService> logger,
        IStringLocalizer<SharedResources> localizer)
    {
        _logger = logger;
        _localizer = localizer;
        var supabaseOptions = options.Value;
        _bucketName = supabaseOptions.BucketName;
        _s3Client = ConfigureS3Client(supabaseOptions.Url, supabaseOptions.PublicKey, supabaseOptions.SecretKey);
    }

    private static IAmazonS3 ConfigureS3Client(string url, string publicKey, string secretKey)
    {
        var s3Config = new AmazonS3Config
        {
            ServiceURL = url,
            ForcePathStyle = true
        };
        return new AmazonS3Client(publicKey, secretKey, s3Config);
    }

    public async Task<string> GenerateSignedUrlAsync(string fileName, int expiresInSeconds, CancellationToken cancellationToken = default)
    {
        Guard.AgainstEmptyString(fileName, nameof(fileName), _localizer.GetString(SharedResources.FileNameRequired));
        Guard.AgainstFalse(expiresInSeconds > 0, nameof(expiresInSeconds), _localizer.GetString(SharedResources.InvalidExpirationTime));

        try
        {
            _logger.LogInformation("Generating signed URL for {FileName} in {BucketName}", fileName, _bucketName);
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                Expires = DateTime.UtcNow.AddSeconds(expiresInSeconds)
            };
            var url = await _s3Client.GetPreSignedURLAsync(request);
            _logger.LogInformation("Generated signed URL for {FileName}", fileName);
            return url;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Failed to generate signed URL for {FileName} in {BucketName}", fileName, _bucketName);
            throw new Exception(_localizer.GetString(SharedResources.FailedToGenerateSignedUrl, fileName).Value, ex);
        }
    }
}

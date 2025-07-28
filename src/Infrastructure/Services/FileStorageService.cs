using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Application.Common;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _publicUrl;
    private readonly ILogger<FileStorageService> _logger;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public FileStorageService(
        IConfiguration configuration,
        ILogger<FileStorageService> logger,
        IStringLocalizer<SharedResources> localizer)
    {
        Guard.AgainstNull(configuration, nameof(configuration), localizer.GetString(SharedResources.ConfigurationRequired));
        Guard.AgainstNull(logger, nameof(logger), localizer.GetString(SharedResources.LoggerRequired));
        Guard.AgainstNull(localizer, nameof(localizer), localizer.GetString(SharedResources.LocalizerRequired));

        _logger = logger;
        _localizer = localizer;

        var (url, publicKey, secretKey, bucketName, publicUrl) = ValidateConfiguration(configuration, localizer);
        _bucketName = bucketName;
        _publicUrl = publicUrl;
        _s3Client = ConfigureS3Client(url, publicKey, secretKey);
    }

    private static (
        string url,
        string publicKey,
        string secretKey,
        string bucketName,
        string publicUrl)
        ValidateConfiguration(
        IConfiguration configuration, IStringLocalizer<SharedResources> localizer)
    {
        var url = configuration["Supabase:Url"];
        var publicKey = configuration["Supabase:PublicKey"];
        var secretKey = configuration["Supabase:SecretKey"];
        var bucketName = configuration["Supabase:BucketName"];
        var publicUrl = configuration["Supabase:PublicUrl"];

        Guard.AgainstEmptyString(url, nameof(url), localizer.GetString(SharedResources.SupabaseUrlRequired));
        Guard.AgainstEmptyString(publicKey, nameof(publicKey), localizer.GetString(SharedResources.SupabasePublicKeyRequired));
        Guard.AgainstEmptyString(secretKey, nameof(secretKey), localizer.GetString(SharedResources.SupabaseSecretKeyRequired));
        Guard.AgainstEmptyString(bucketName, nameof(bucketName), localizer.GetString(SharedResources.SupabaseBucketNameRequired));
        Guard.AgainstEmptyString(publicUrl, nameof(publicUrl), localizer.GetString(SharedResources.SupabasePublicUrlRequired));

        return (url, publicKey, secretKey, bucketName, publicUrl);
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

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking bucket {BucketName}", _bucketName);
        var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _bucketName);
        if (!bucketExists) {
            _logger.LogError("Bucket {BucketName} not found", _bucketName);
            throw new InvalidOperationException(string.Format(_localizer.GetString(SharedResources.BucketNotFound), _bucketName));
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken)
    {
        ValidateFileInput(fileStream, fileName);

        await InitializeAsync(cancellationToken);

        var uniqueFileName = GenerateUniqueFileName(fileName);
        var putObjectRequest = CreatePutObjectRequest(uniqueFileName, fileStream);

        await UploadFileToS3Async(putObjectRequest, uniqueFileName, cancellationToken);

        return GenerateFileUrl(uniqueFileName);
    }

    private void ValidateFileInput(Stream fileStream, string fileName)
    {
        Guard.AgainstNull(fileStream, nameof(fileStream), _localizer.GetString(SharedResources.FileStreamRequired));
        Guard.AgainstEmptyString(fileName, nameof(fileName), _localizer.GetString(SharedResources.FileNameRequired));
    }

    private string GenerateUniqueFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var uniqueFileName = $"public/{Guid.NewGuid()}{extension}";
        _logger.LogInformation("Generated file name {UniqueFileName} for {FileName}", uniqueFileName, fileName);
        return uniqueFileName;
    }

    private PutObjectRequest CreatePutObjectRequest(string uniqueFileName, Stream fileStream)
    {
        return new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = uniqueFileName,
            InputStream = fileStream,
            CannedACL = S3CannedACL.PublicRead
        };
    }

    private async Task UploadFileToS3Async(PutObjectRequest putObjectRequest, string uniqueFileName, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Uploading {UniqueFileName} to {BucketName}", uniqueFileName, _bucketName);
            await _s3Client.PutObjectAsync(putObjectRequest, cancellationToken);
            _logger.LogInformation("Uploaded {UniqueFileName}", uniqueFileName);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Failed to upload {UniqueFileName} to {BucketName}", uniqueFileName, _bucketName);
            throw new Exception($"Failed to upload file {uniqueFileName}", ex);
        }
    }

    private string GenerateFileUrl(string uniqueFileName)
    {
        var fileUrl = $"{_publicUrl}/{_bucketName}/{uniqueFileName}";
        _logger.LogInformation("Generated URL: {FileUrl}", fileUrl);
        return fileUrl;
    }

    public async Task DeleteFileAsync(string fileName, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyString(fileName, nameof(fileName), _localizer.GetString(SharedResources.FileNameRequired));

        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName
        };

        try {
            _logger.LogInformation("Deleting {FileName} from {BucketName}", fileName, _bucketName);
            await _s3Client.DeleteObjectAsync(deleteObjectRequest, cancellationToken);
            _logger.LogInformation("Deleted {FileName}", fileName);

        } 
        catch (AmazonS3Exception ex) {
            _logger.LogError(ex, "Failed to delete {FileName} from {BucketName}", fileName, _bucketName);
            throw new Exception($"Failed to delete file {fileName}", ex);
        }
    }
}
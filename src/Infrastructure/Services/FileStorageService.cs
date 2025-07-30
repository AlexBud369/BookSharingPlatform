using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Application.Common;
using Application.Common.Resources;
using Infrastructure.Configuration; 
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
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
        IOptions<SupabaseOptions> options,
        ILogger<FileStorageService> logger,
        IStringLocalizer<SharedResources> localizer)
    {
        _logger = logger;
        _localizer = localizer;
        var supabaseOptions = options.Value;

        Validator.ValidateObject(supabaseOptions, new ValidationContext(supabaseOptions), validateAllProperties: true);

        _bucketName = supabaseOptions.BucketName;
        _publicUrl = supabaseOptions.PublicUrl;
        _s3Client = ConfigureS3Client(supabaseOptions.Url, supabaseOptions.PublicKey, supabaseOptions.SecretKey);
    }

    private static IAmazonS3 ConfigureS3Client(string url, string publicKey, string secretKey)
    {
        var s3Config = new AmazonS3Config {
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
            throw new InvalidOperationException(_localizer[SharedResources.BucketNotFound, _bucketName].Value);
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
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
        if (fileStream == null) {
            throw new ArgumentNullException(
                nameof(fileStream),
                _localizer[SharedResources.FileStreamRequired].Value);
        }

        if (string.IsNullOrEmpty(fileName)) {
            throw new ArgumentNullException(
                nameof(fileName),
                _localizer[SharedResources.FileNameRequired].Value);
        }
            
    }

    private string GenerateUniqueFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var uniqueFileName = $"private/{Guid.NewGuid()}{extension}";
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
            CannedACL = S3CannedACL.Private
        };
    }

    private async Task UploadFileToS3Async(PutObjectRequest putObjectRequest, string uniqueFileName, CancellationToken cancellationToken)
    {
        try {
            _logger.LogInformation("Uploading {UniqueFileName} to {BucketName}", uniqueFileName, _bucketName);
            await _s3Client.PutObjectAsync(putObjectRequest, cancellationToken);
            _logger.LogInformation("Uploaded {UniqueFileName}", uniqueFileName);

        } catch (AmazonS3Exception ex) {
            _logger.LogError(ex, "Failed to upload {UniqueFileName} to {BucketName}", uniqueFileName, _bucketName);
            throw new Exception(_localizer[SharedResources.FailedToUploadFile, uniqueFileName].Value, ex);
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
        if (string.IsNullOrEmpty(fileName)) {
            throw new ArgumentNullException(
                nameof(fileName),
                _localizer[SharedResources.FileNameRequired].Value);
        }

        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName
        };

        try {
            _logger.LogInformation("Deleting {FileName} from {BucketName}", fileName, _bucketName);
            await _s3Client.DeleteObjectAsync(deleteObjectRequest, cancellationToken);
            _logger.LogInformation("Deleted {FileName}", fileName);

        } catch (AmazonS3Exception ex) {
            _logger.LogError(ex, "Failed to delete {FileName} from {BucketName}", fileName, _bucketName);
            throw new Exception(_localizer[SharedResources.FailedToDeleteFile, fileName].Value, ex);
        }
    }

    
}

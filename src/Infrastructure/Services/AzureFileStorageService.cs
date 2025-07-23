using Application.Common;
using Application.Interfaces;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;

namespace Infrastructure.Services;
public class AzureFileStorageService : IFileStorageService
{
    private readonly BlobContainerClient _containerClient;

    public AzureFileStorageService(IConfiguration configuration)
    {
        var blobServiceClient = new BlobServiceClient(configuration["AzureBlob:ConnectionString"]);
        _containerClient = blobServiceClient.GetBlobContainerClient(configuration["AzureBlob:ContainerName"]);
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await _containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        await _containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob, cancellationToken: cancellationToken);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(fileStream, nameof(fileStream), "FileStreamRequired");
        Guard.AgainstEmptyString(fileName, nameof(fileName), "FileNameRequired");

        await InitializeAsync(cancellationToken);
        var blobClient = _containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, true, cancellationToken);
        return blobClient.Uri.ToString();
    }

    public async Task DeleteFileAsync(string fileName, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyString(fileName, nameof(fileName), "FileNameRequired");
        var blobClient = _containerClient.GetBlobClient(fileName);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}
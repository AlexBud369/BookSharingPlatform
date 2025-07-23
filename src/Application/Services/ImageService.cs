using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Localization;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services;

public class ImageService : IImageService
{
    private readonly IFileStorageService _storageService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public ImageService(
        IFileStorageService storageService,
        IStringLocalizer<SharedResource> localizer)
    {
        _storageService = storageService;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    public async Task<string> UpdateBookCoverAsync(
        Book book,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken,
        bool deleteOldCover = true)
    {
        Guard.AgainstNull(book, nameof(book), "BookNotFound");
        Guard.AgainstNull(fileStream, nameof(fileStream), "FileStreamRequired");
        Guard.AgainstEmptyString(fileName, nameof(fileName), "FileNameRequired");

        var uniqueFileName = $"{book.Id}_{Guid.NewGuid()}{Path.GetExtension(fileName).ToLowerInvariant()}";

        if (deleteOldCover && !string.IsNullOrEmpty(book.CoverImageUrl)) {
            await DeleteBookCoverAsync(book, cancellationToken);
        }

        var imageUrl = await _storageService.UploadFileAsync(fileStream, uniqueFileName, cancellationToken);
        book.CoverImageUrl = imageUrl;
        return imageUrl;
    }

    public async Task DeleteBookCoverAsync(Book book, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(book, nameof(book), "BookNotFound");
        if (!string.IsNullOrEmpty(book.CoverImageUrl)) {
            var fileName = Path.GetFileName(new Uri(book.CoverImageUrl).LocalPath);
            await _storageService.DeleteFileAsync(fileName, cancellationToken);
            book.CoverImageUrl = null;
        }
    }
}
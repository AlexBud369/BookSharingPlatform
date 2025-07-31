using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Microsoft.Extensions.Localization;

namespace Application.Services;

public class ImageService : IImageService
{
    private readonly IFileStorageService _storageService;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public ImageService(
        IFileStorageService storageService,
        IStringLocalizer<SharedResources> localizer)
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
        Guard.AgainstNull(book, nameof(book), _localizer.GetString(SharedResources.BookNotFound));
        Guard.AgainstEmptyGuid(book.Id, nameof(book.Id), _localizer.GetString(SharedResources.BookIdRequired));
        Guard.AgainstNull(fileStream, nameof(fileStream), _localizer.GetString(SharedResources.FileStreamRequired));
        Guard.AgainstEmptyString(fileName, nameof(fileName), _localizer.GetString(SharedResources.FileNameRequired));

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        Guard.AgainstFalse(!DomainConstants.BookCover.AllowedImageExtensions.Contains(extension), nameof(fileName), _localizer.GetString(SharedResources.InvalidFileExtension), string.Join(", ", DomainConstants.BookCover.AllowedImageExtensions));

        var uniqueFileName = $"{book.Id}_{Guid.NewGuid()}{extension}";

        if (deleteOldCover && !string.IsNullOrEmpty(book.CoverImageUrl)) {
            await DeleteBookCoverAsync(book, cancellationToken);
        }

        var imageUrl = await _storageService.UploadFileAsync(fileStream, uniqueFileName, cancellationToken);
        book.CoverImageUrl = imageUrl;
        return imageUrl;
    }

    public async Task DeleteBookCoverAsync(Book book, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(book, nameof(book), _localizer.GetString(SharedResources.BookNotFound));
        Guard.AgainstEmptyGuid(book.Id, nameof(book.Id), _localizer.GetString(SharedResources.BookIdRequired));

        if (!string.IsNullOrEmpty(book.CoverImageUrl)) {
            var fileName = Path.GetFileName(new Uri(book.CoverImageUrl).LocalPath);
            await _storageService.DeleteFileAsync(fileName, cancellationToken);
            book.CoverImageUrl = null;
        }
    }
}
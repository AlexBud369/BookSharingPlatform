using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Localization;

namespace Application.Services;

public class ImageService : IImageService
{
    private readonly IStorageService _storageService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public ImageService(
        IStorageService storageService,
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
        CancellationToken cancellationToken)
    {
        Guard.AgainstNull(book, nameof(book), "BookNotFound");
        Guard.AgainstNull(fileStream, nameof(fileStream), "FileStreamRequired");
        Guard.AgainstEmptyString(fileName, nameof(fileName), "FileNameRequired");

        var imageUrl = await _storageService.UploadFileAsync(fileStream, fileName, cancellationToken);
        book.CoverImageUrl = imageUrl;

        return imageUrl;
    }
}
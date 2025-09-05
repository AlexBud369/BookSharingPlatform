using Application.Common;
using Application.Features.Books.Commands;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System.IO;

namespace Application.Validators.Books;

public class UploadBookCoverCommandValidator : AbstractValidator<UploadBookCover.Command>
{
    public UploadBookCoverCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.BookIdRequired));

        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.UserIdRequired));

        RuleFor(x => x.CoverImage)
            .NotNull().WithMessage(localizer.GetString(SharedResources.CoverImageRequired))
            .Must(file => file.Length > 0).WithMessage(localizer.GetString(SharedResources.CoverImageEmpty))
            .Must(file => file.Length <= DomainConstants.BookCover.MaxFileSizeBytes)
            .WithMessage(localizer.GetString(SharedResources.CoverImageTooLarge, DomainConstants.BookCover.MaxFileSizeBytes / 1024 / 1024))
            .Must(file => DomainConstants.BookCover.AllowedImageExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()))
            .WithMessage(localizer.GetString(SharedResources.InvalidFileExtension, string.Join(", ", DomainConstants.BookCover.AllowedImageExtensions)));
    }
}
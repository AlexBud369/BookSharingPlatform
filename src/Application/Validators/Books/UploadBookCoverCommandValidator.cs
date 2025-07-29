using Application.Common;
using Application.Features.Books.Commands;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;

namespace Application.Validators.Books;

public class UploadBookCoverCommandValidator : AbstractValidator<UploadBookCoverCommand>
{
    public UploadBookCoverCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.BookId)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.Book.Required.BookIdRequired));

        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.User.Required.UserIdRequired));

        RuleFor(x => x.CoverImage)
            .NotNull().WithMessage(localizer.GetString(SharedResources.Book.Required.CoverImageRequired))
            .Must(file => file.Length > 0).WithMessage(localizer.GetString(SharedResources.Book.Required.CoverImageEmpty))
            .Must(file => file.Length <= DomainConstants.BookCover.MaxFileSizeBytes)
            .WithMessage(localizer.GetString(SharedResources.Book.Length.CoverImageTooLarge, DomainConstants.BookCover.MaxFileSizeBytes / 1024 / 1024))
            .Must(file => DomainConstants.BookCover.AllowedImageExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()))
            .WithMessage(localizer.GetString(SharedResources.Common.Validation.InvalidFileExtension) + $": {string.Join(", ", DomainConstants.BookCover.AllowedImageExtensions)}");
    }
}
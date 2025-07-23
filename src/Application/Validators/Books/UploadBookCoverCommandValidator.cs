using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Features.Books.Commands;


namespace Application.Validators.Books;

public class UploadBookCoverCommandValidator : AbstractValidator<UploadBookCoverCommand>
{
    public UploadBookCoverCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.Id)
           .NotEmpty().WithMessage(localizer["BookIdRequired"]);

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(localizer["UserIdRequired"]);

        RuleFor(x => x.CoverImage)
            .NotNull().WithMessage(localizer["CoverImageRequired"])
            .Must(file => file.Length > 0).WithMessage(localizer["CoverImageRequired"])
            .Must(file => file.Length <= DomainConstants.BookCover.MaxFileSizeBytes)
            .WithMessage(localizer["CoverImageTooLarge", DomainConstants.BookCover.MaxFileSizeBytes / 1024 / 1024])
            .Must(file => new[] { ".jpg", ".jpeg", ".png" }.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()))
            .WithMessage(localizer["InvalidCoverImageFormat"]);
    }
}
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

        RuleFor(x => x.CoverImageUrl)
            .NotEmpty().WithMessage(localizer["CoverImageUrlRequired"])
            .MaximumLength(DomainConstants.BookCover.UrlMaxLength).WithMessage(localizer["CoverImageUrlTooLong"])
            .Matches(@"^https?://").WithMessage(localizer["InvalidCoverImageUrl"]);
    }
}
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Features.Books.Commands;

namespace Application.Validators.Books;

public class UpdateBookCommandValidator : AbstractValidator<BookUpdateDto>
{
    public UpdateBookCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(localizer["BookIdRequired"]);

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(localizer["UserIdRequired"]);

        RuleFor(x => x.Book)
            .NotNull().WithMessage(localizer["BookDataRequired"]);

        RuleFor(x => x.Book)
            .Must(dto => dto.Title != null || dto.Author != null || dto.Description != null ||
                         dto.CoverImageUrl != null || dto.Tags != null)
            .WithMessage(localizer["AtLeastOneFieldRequired"])
            .When(x => x.Book != null);

        RuleFor(x => x.Book.Title)
            .NotEmpty().WithMessage(localizer["TitleRequired"])
            .MaximumLength(DomainConstants.Book.TitleMaxLength).WithMessage(localizer["TitleTooLong"])
            .When(x => x.Book != null && x.Book.Title != null);

        RuleFor(x => x.Book.Author)
            .NotEmpty().WithMessage(localizer["AuthorRequired"])
            .MaximumLength(DomainConstants.Book.AuthorMaxLength).WithMessage(localizer["AuthorTooLong"])
            .When(x => x.Book != null && x.Book.Author != null);

        RuleFor(x => x.Book.Description)
            .MaximumLength(DomainConstants.Book.DescriptionMaxLength).WithMessage(localizer["DescriptionTooLong"])
            .When(x => x.Book != null && x.Book.Description != null);

        RuleFor(x => x.Book.CoverImageUrl)
            .MaximumLength(DomainConstants.BookCover.UrlMaxLength).WithMessage(localizer["CoverImageUrlTooLong"])
            .When(x => x.Book != null && x.Book.CoverImageUrl != null);

        RuleFor(x => x.Book.Tags)
            .Must(tags => tags == null || tags.All(id => id != Guid.Empty)).WithMessage(localizer["InvalidTagIds"])
            .When(x => x.Book != null && x.Book.Tags != null);
    }
}

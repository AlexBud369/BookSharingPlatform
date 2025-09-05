using Application.Common;
using Application.DTOs.Book;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Books;

public class BookUpdateDtoValidator : AbstractValidator<BookUpdateDto>
{
    public BookUpdateDtoValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.TitleRequired))
            .MaximumLength(DomainConstants.Book.TitleMaxLength).WithMessage(localizer.GetString(SharedResources.TitleTooLong))
            .When(x => x.Title != null);

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.AuthorRequired))
            .MaximumLength(DomainConstants.Book.AuthorMaxLength).WithMessage(localizer.GetString(SharedResources.AuthorTooLong))
            .When(x => x.Author != null);

        RuleFor(x => x.Description)
            .MaximumLength(DomainConstants.Book.DescriptionMaxLength).WithMessage(localizer.GetString(SharedResources.DescriptionTooLong))
            .When(x => x.Description != null);

        RuleFor(x => x.CoverImageUrl)
            .MaximumLength(DomainConstants.BookCover.UrlMaxLength).WithMessage(localizer.GetString(SharedResources.CoverImageUrlTooLong))
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _)).WithMessage(localizer.GetString(SharedResources.InvalidCoverImageUrl))
            .When(x => x.CoverImageUrl != null);

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.All(id => id != Guid.Empty)).WithMessage(localizer.GetString(SharedResources.InvalidTagIds))
            .When(x => x.Tags != null);

        RuleFor(x => x)
            .Must(dto => dto.Title != null || dto.Author != null || dto.Description != null || dto.CoverImageUrl != null || dto.Tags != null)
            .WithMessage(localizer.GetString(SharedResources.AtLeastOneFieldRequired));
    }
}
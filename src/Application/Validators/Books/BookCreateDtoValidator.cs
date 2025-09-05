using Application.Common;
using Application.DTOs.Book;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Books;

public class BookCreateDtoValidator : AbstractValidator<BookCreateDto>
{
    public BookCreateDtoValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.TitleRequired))
            .MaximumLength(DomainConstants.Book.TitleMaxLength).WithMessage(localizer.GetString(SharedResources.TitleTooLong));

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.AuthorRequired))
            .MaximumLength(DomainConstants.Book.AuthorMaxLength).WithMessage(localizer.GetString(SharedResources.AuthorTooLong));

        RuleFor(x => x.Description)
            .MaximumLength(DomainConstants.Book.DescriptionMaxLength).WithMessage(localizer.GetString(SharedResources.DescriptionTooLong))
            .When(x => x.Description != null);

        RuleFor(x => x.Tags)
            .NotNull().WithMessage(localizer.GetString(SharedResources.TagsRequired))
            .Must(tags => tags.All(id => id != Guid.Empty)).WithMessage(localizer.GetString(SharedResources.InvalidTagIds));
    }
}
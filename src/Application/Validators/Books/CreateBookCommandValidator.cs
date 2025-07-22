using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Features.Books.Commands;

namespace Application.Validators.Books;

public class CreateBookCommandValidator : AbstractValidator<BookCreateDto>
{
    public CreateBookCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(b => b.UserId)
             .NotEmpty().WithMessage(localizer["UserIdRequired"]);

        RuleFor(b => b.Book)
            .NotNull().WithMessage(localizer["BookDataRequired"]);

        RuleFor(b => b.Book.Title)
            .NotEmpty().WithMessage(localizer["TitleRequired"])
            .MaximumLength(DomainConstants.Book.TitleMaxLength)
            .WithMessage(localizer["TitleTooLong"])
            .When(b => b.Book != null);

        RuleFor(b => b.Book.Author)
            .NotEmpty().WithMessage(localizer["AuthorRequired"])
            .MaximumLength(DomainConstants.Book.AuthorMaxLength)
            .WithMessage(localizer["AuthorTooLong"])
            .When(b => b.Book != null);

        RuleFor(b => b.Book.Description)
            .MaximumLength(DomainConstants.Book.DescriptionMaxLength)
            .WithMessage(localizer["DescriptionTooLong"])
            .When(b => b.Book != null && b.Book.Description != null);

        RuleFor(b => b.Book.Tags)
            .NotNull().WithMessage(localizer["TagsRequired"])
            .Must(tags => tags.All(id => id != Guid.Empty)).WithMessage(localizer["InvalidTagIds"])
            .When(x => x.Book != null);

    }
}

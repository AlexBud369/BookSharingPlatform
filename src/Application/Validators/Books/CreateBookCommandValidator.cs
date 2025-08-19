using Application.Common;
using Application.Features.Books.Commands;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;

namespace Application.Validators.Books;

public class CreateBookCommandValidator : AbstractValidator<CreateBook.Command>
{
    public CreateBookCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.UserIdRequired));

        RuleFor(x => x.Book)
            .NotNull().WithMessage(localizer.GetString(SharedResources.BookDataRequired));

        RuleFor(x => x.Book.Title)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.TitleRequired))
            .MaximumLength(DomainConstants.Book.TitleMaxLength).WithMessage(localizer.GetString(SharedResources.TitleTooLong))
            .When(x => x.Book != null);

        RuleFor(x => x.Book.Author)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.AuthorRequired))
            .MaximumLength(DomainConstants.Book.AuthorMaxLength).WithMessage(localizer.GetString(SharedResources.AuthorTooLong))
            .When(x => x.Book != null);

        RuleFor(x => x.Book.Description)
            .MaximumLength(DomainConstants.Book.DescriptionMaxLength).WithMessage(localizer.GetString(SharedResources.DescriptionTooLong))
            .When(x => x.Book != null && x.Book.Description != null);

        RuleFor(x => x.Book.Tags)
            .NotNull().WithMessage(localizer.GetString(SharedResources.TagsRequired))
            .Must(tags => tags.All(id => id != Guid.Empty)).WithMessage(localizer.GetString(SharedResources.InvalidTagIds))
            .When(x => x.Book != null);
    }
}
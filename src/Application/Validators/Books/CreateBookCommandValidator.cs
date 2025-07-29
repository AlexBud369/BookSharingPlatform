using Application.Common;
using Application.Features.Books.Commands;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;

namespace Application.Validators.Books;

public class CreateBookCommandValidator : AbstractValidator<BookCreateDto>
{
    public CreateBookCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(b => b.UserId)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.UserIdRequired));

        RuleFor(b => b.Book)
            .NotNull().WithMessage(localizer.GetString(SharedResources.BookDataRequired));

        RuleFor(b => b.Book.Title)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.TitleRequired))
            .MaximumLength(DomainConstants.Book.TitleMaxLength)
            .WithMessage(localizer.GetString(SharedResources.TitleTooLong))
            .When(b => b.Book != null);

        RuleFor(b => b.Book.Author)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.AuthorRequired))
            .MaximumLength(DomainConstants.Book.AuthorMaxLength)
            .WithMessage(localizer.GetString(SharedResources.AuthorTooLong))
            .When(b => b.Book != null);

        RuleFor(b => b.Book.Description)
            .MaximumLength(DomainConstants.Book.DescriptionMaxLength)
            .WithMessage(localizer.GetString(SharedResources.DescriptionTooLong))
            .When(b => b.Book != null && b.Book.Description != null);

        RuleFor(b => b.Book.Tags)
            .NotNull().WithMessage(localizer.GetString(SharedResources.TagsRequired))
            .Must(tags => tags.All(id => id != Guid.Empty)).WithMessage(localizer.GetString(SharedResources.InvalidTagIds))
            .When(x => x.Book != null);
    }
}
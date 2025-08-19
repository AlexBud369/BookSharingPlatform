using Application.Common;
using Application.Features.Books.Commands;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;

namespace Application.Validators.Books;

public class UpdateBookCommandValidator : AbstractValidator<UpdateBook.Command>
{
    public UpdateBookCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.BookIdRequired));

        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.UserIdRequired));

        RuleFor(x => x.Book)
            .NotNull().WithMessage(localizer.GetString(SharedResources.BookDataRequired));

        RuleFor(x => x.Book.Title)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.TitleRequired))
            .MaximumLength(DomainConstants.Book.TitleMaxLength).WithMessage(localizer.GetString(SharedResources.TitleTooLong))
            .When(x => x.Book != null && x.Book.Title != null);

        RuleFor(x => x.Book.Author)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.AuthorRequired))
            .MaximumLength(DomainConstants.Book.AuthorMaxLength).WithMessage(localizer.GetString(SharedResources.AuthorTooLong))
            .When(x => x.Book != null && x.Book.Author != null);

        RuleFor(x => x.Book.Description)
            .MaximumLength(DomainConstants.Book.DescriptionMaxLength).WithMessage(localizer.GetString(SharedResources.DescriptionTooLong))
            .When(x => x.Book != null && x.Book.Description != null);

        RuleFor(x => x.Book.CoverImageUrl)
            .MaximumLength(DomainConstants.BookCover.UrlMaxLength).WithMessage(localizer.GetString(SharedResources.CoverImageUrlTooLong))
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _)).WithMessage(localizer.GetString(SharedResources.InvalidCoverImageUrl))
            .When(x => x.Book != null && x.Book.CoverImageUrl != null);

        RuleFor(x => x.Book.Tags)
            .Must(tags => tags == null || tags.All(id => id != Guid.Empty)).WithMessage(localizer.GetString(SharedResources.InvalidTagIds))
            .When(x => x.Book != null && x.Book.Tags != null);

        RuleFor(x => x.Book)
            .Must(dto => dto.Title != null || dto.Author != null || dto.Description != null || dto.CoverImageUrl != null || dto.Tags != null)
            .WithMessage(localizer.GetString(SharedResources.AtLeastOneFieldRequired))
            .When(x => x.Book != null);
    }
}
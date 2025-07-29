using Application.Common;
using Application.Common.Enums;
using Application.Features.Books.Queries;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;

namespace Application.Validators.Books;

public class GetAllBooksQueryValidator : AbstractValidator<GetAllBooksQuery>
{
    public GetAllBooksQueryValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Filter)
            .NotNull().WithMessage(localizer.GetString(SharedResources.Common.Required.FilterRequired));

        RuleFor(x => x.Filter.PageNumber)
            .GreaterThanOrEqualTo(DomainConstants.Book.DefaultPageNumber).WithMessage(localizer.GetString(SharedResources.Common.Validation.InvalidPageNumber));

        RuleFor(x => x.Filter.PageSize)
            .InclusiveBetween(DomainConstants.Book.DefaultPageSize, DomainConstants.Book.MaxPageSize).WithMessage(localizer.GetString(SharedResources.Common.Validation.InvalidPageSize));

        RuleFor(x => x.Filter.SearchQuery)
            .MaximumLength(DomainConstants.Book.TitleMaxLength).WithMessage(localizer.GetString(SharedResources.Common.Validation.SearchQueryTooLong))
            .When(x => !string.IsNullOrEmpty(x.Filter.SearchQuery));

        RuleFor(x => x.Filter.TagIds)
            .Must(tags => tags == null || tags.All(id => id != Guid.Empty)).WithMessage(localizer.GetString(SharedResources.Tag.Validation.InvalidTagIds))
            .When(x => x.Filter.TagIds != null);

        RuleFor(x => x.Filter.SortBy)
            .Must(x => x == null || new[] { "createdAt", "title" }.Contains(x.ToLowerInvariant()))
            .WithMessage(localizer.GetString(SharedResources.Common.Validation.InvalidSortBy))
            .When(x => !string.IsNullOrEmpty(x.Filter.SortBy));
    }
}
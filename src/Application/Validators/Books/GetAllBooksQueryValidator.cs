using Application.Common;
using Application.Common.Enums;
using Application.Features.Books.Queries;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Books;

public class GetAllBooksQueryValidator : AbstractValidator<GetAllBooks.Query>
{
    public GetAllBooksQueryValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Filter)
            .NotNull().WithMessage(localizer.GetString(SharedResources.FilterRequired));

        RuleFor(x => x.Filter.PageNumber)
            .GreaterThanOrEqualTo(DomainConstants.Book.DefaultPageNumber).WithMessage(localizer.GetString(SharedResources.InvalidPageNumber));

        RuleFor(x => x.Filter.PageSize)
            .InclusiveBetween(DomainConstants.Book.DefaultPageSize, DomainConstants.Book.MaxPageSize).WithMessage(localizer.GetString(SharedResources.InvalidPageSize));

        RuleFor(x => x.Filter.SearchQuery)
            .MaximumLength(DomainConstants.Book.SearchQueryMaxLength).WithMessage(localizer.GetString(SharedResources.SearchQueryTooLong))
            .When(x => !string.IsNullOrEmpty(x.Filter.SearchQuery));

        RuleFor(x => x.Filter.TagIds)
            .Must(tags => tags == null || tags.All(id => id != Guid.Empty)).WithMessage(localizer.GetString(SharedResources.InvalidTagIds))
            .When(x => x.Filter.TagIds != null);

        RuleFor(x => x.Filter.SortBy)
            .Must(x => x == null || new[] { "createdAt", "title" }.Contains(x.ToLowerInvariant()))
            .WithMessage(localizer.GetString(SharedResources.InvalidSortBy))
            .When(x => !string.IsNullOrEmpty(x.Filter.SortBy));
    }
}
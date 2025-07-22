using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Features.Books.Queries;


namespace Application.Validators.Books;

public class GetAllBooksQueryValidator : AbstractValidator<GetAllBooksQuery>
{
    public GetAllBooksQueryValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.Filter)
           .NotNull().WithMessage(localizer["FilterRequired"]);

        RuleFor(x => x.Filter.PageNumber)
            .GreaterThanOrEqualTo(DomainConstants.Book.DefaultPageNumber).WithMessage(localizer["InvalidPageNumber"]);

        RuleFor(x => x.Filter.PageSize)
            .InclusiveBetween(1, DomainConstants.Book.MaxPageSize).WithMessage(localizer["InvalidPageSize"]);

        RuleFor(x => x.Filter.SearchQuery)
            .MaximumLength(DomainConstants.Book.TitleMaxLength).WithMessage(localizer["SearchQueryTooLong"])
            .When(x => !string.IsNullOrEmpty(x.Filter.SearchQuery));

        RuleFor(x => x.Filter.TagIds)
            .Must(tags => tags == null || tags.All(id => id != Guid.Empty)).WithMessage(localizer["InvalidTagIds"])
            .When(x => x.Filter.TagIds != null);

        RuleFor(x => x.Filter.SortBy)
            .Must(x => x == null || x == DomainConstants.Book.SortByCreatedAt || x == DomainConstants.Book.SortByTitle)
            .WithMessage(localizer["InvalidSortBy"])
            .When(x => !string.IsNullOrEmpty(x.Filter.SortBy));
    }
}
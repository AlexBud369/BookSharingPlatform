using Application.Common;
using Application.DTOs.Book;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Books;

public class BookFilterDtoValidator : AbstractValidator<BookFilterDto>
{
    public BookFilterDtoValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(DomainConstants.Book.DefaultPageNumber).WithMessage(localizer.GetString(SharedResources.InvalidPageNumber));

        RuleFor(x => x.PageSize)
            .InclusiveBetween(DomainConstants.Book.DefaultPageSize, DomainConstants.Book.MaxPageSize).WithMessage(localizer.GetString(SharedResources.InvalidPageSize, DomainConstants.Book.MaxPageSize));

        RuleFor(x => x.SearchQuery)
            .MaximumLength(DomainConstants.Book.SearchQueryMaxLength).WithMessage(localizer.GetString(SharedResources.SearchQueryTooLong))
            .When(x => x.SearchQuery != null);

        RuleFor(x => x.CreatedByUserId)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.UserIdRequired))
            .When(x => x.CreatedByUserId.HasValue);
    }
}
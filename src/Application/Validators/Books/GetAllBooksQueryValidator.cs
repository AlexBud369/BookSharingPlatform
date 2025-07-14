using FluentValidation;
using Application.Features.Books.Queries;

namespace Application.Validators.Books;

public class GetAllBooksQueryValidator : AbstractValidator<GetAllBooksQuery>
{
    public GetAllBooksQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Author)
            .MaximumLength(100).WithMessage("Author must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Author));

        RuleFor(x => x.TagNames)
            .Must(tags => tags.All(tag => !string.IsNullOrEmpty(tag)))
            .WithMessage("All tag names must be non-empty")
            .When(x => x.TagNames != null);

        RuleFor(x => x.SortBy)
            .Must(x => x == null || x == "CreatedAt" || x == "Title")
            .WithMessage("SortBy must be 'CreatedAt' or 'Title'")
            .When(x => !string.IsNullOrEmpty(x.SortBy));
    }
}
using FluentValidation;
using Application.Features.Tags.Queries;

namespace Application.Validators.Tags;

public class GetAllTagsQueryValidator : AbstractValidator<GetAllTagsQuery>
{
    public GetAllTagsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.TagName)
            .MaximumLength(50).WithMessage("Tag name must not exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9\s-]*$")
            .WithMessage("Tag name can only contain letters, numbers, spaces, and hyphens")
            .When(x => !string.IsNullOrEmpty(x.TagName));
    }
}
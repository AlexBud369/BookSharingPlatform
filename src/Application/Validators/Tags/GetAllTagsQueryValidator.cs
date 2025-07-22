using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Features.Tags.Queries;

namespace Application.Validators.Tags;

public class GetAllTagsQueryValidator : AbstractValidator<GetAllTagsQuery>
{
    public GetAllTagsQueryValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.PageNumber)
           .GreaterThanOrEqualTo(DomainConstants.Book.DefaultPageNumber).WithMessage(localizer["InvalidPageNumber"]);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, DomainConstants.Book.MaxPageSize).WithMessage(localizer["InvalidPageSize"]);

        RuleFor(x => x.TagName)
            .MaximumLength(DomainConstants.Tag.NameMaxLength).WithMessage(localizer["TagNameTooLong"])
            .Matches(@"^[a-zA-Z0-9\s-]*$").WithMessage(localizer["InvalidTagNameFormat"])
            .When(x => !string.IsNullOrEmpty(x.TagName));
    }
}
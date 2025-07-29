using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Common;
using Application.Features.Tags.Queries;

namespace Application.Validators.Tags;

public class GetAllTagsQueryValidator : AbstractValidator<GetAllTagsQuery>
{
    public GetAllTagsQueryValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(DomainConstants.Tag.DefaultPageNumber).WithMessage(localizer.GetString(SharedResources.Common.Validation.InvalidPageNumber));

        RuleFor(x => x.PageSize)
            .InclusiveBetween(DomainConstants.Tag.DefaultPageSize, DomainConstants.Tag.MaxPageSize).WithMessage(localizer.GetString(SharedResources.Common.Validation.InvalidPageSize));

        RuleFor(x => x.TagName)
            .MaximumLength(DomainConstants.Tag.NameMaxLength).WithMessage(localizer.GetString(SharedResources.Tag.Length.TagNameTooLong))
            .Matches(@"^[a-zA-Z0-9\s-]*$").WithMessage(localizer.GetString(SharedResources.Tag.Validation.InvalidTagNameFormat))
            .When(x => !string.IsNullOrEmpty(x.TagName));
    }
}
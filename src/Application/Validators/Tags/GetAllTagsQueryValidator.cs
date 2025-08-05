using Application.Common;
using Application.Features.Tags.Queries;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Tags;

public class GetAllTagsQueryValidator : AbstractValidator<GetAllTagsQuery>
{
    public GetAllTagsQueryValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(DomainConstants.Tag.MinPageNumber).WithMessage(localizer.GetString(SharedResources.InvalidPageNumber));

        RuleFor(x => x.PageSize)
            .InclusiveBetween(DomainConstants.Tag.DefaultPageSize, DomainConstants.Tag.MaxPageSize).WithMessage(localizer.GetString(SharedResources.InvalidPageSize));

        RuleFor(x => x.TagName)
            .MaximumLength(DomainConstants.Tag.NameMaxLength).WithMessage(localizer.GetString(SharedResources.TagNameTooLong))
            .Matches(@"^[a-zA-Z0-9\s-]*$").WithMessage(localizer.GetString(SharedResources.InvalidTagNameFormat))
            .When(x => !string.IsNullOrEmpty(x.TagName));
    }
}
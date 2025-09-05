using Application.Common;
using Application.Features.Tags.Queries;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Tags;

public class GetAllTagsValidator : AbstractValidator<GetAllTags>
{
    public GetAllTagsValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(DomainConstants.Tag.MinPageNumber)
            .WithMessage(localizer[SharedResources.InvalidPageNumber]);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(DomainConstants.Tag.DefaultPageSize, DomainConstants.Tag.MaxPageSize)
            .WithMessage(localizer[SharedResources.InvalidPageSize]);

        RuleFor(x => x.TagName)
            .MaximumLength(DomainConstants.Tag.NameMaxLength)
            .WithMessage(localizer[SharedResources.TagNameTooLong])
            .Matches(@"^[a-zA-Z0-9\s-]*$")
            .WithMessage(localizer[SharedResources.InvalidTagNameFormat])
            .When(x => !string.IsNullOrEmpty(x.TagName));
    }
}
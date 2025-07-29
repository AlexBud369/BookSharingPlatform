using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Common;
using Application.Features.Tags.Queries;

namespace Application.Validators.Tags;

public class GetTagByIdQueryValidator : AbstractValidator<GetTagByIdQuery>
{
    public GetTagByIdQueryValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.Tag.Required.TagIdRequired));
    }
}
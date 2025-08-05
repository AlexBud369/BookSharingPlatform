using Application.Common;
using Application.Features.Tags.Queries;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Tags;

public class GetTagByIdQueryValidator : AbstractValidator<GetTagByIdQuery>
{
    public GetTagByIdQueryValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.TagIdRequired));
    }
}
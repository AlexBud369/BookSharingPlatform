using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Features.Tags.Queries;

namespace Application.Validators.Tags;

public class GetTagByIdQueryValidator : AbstractValidator<GetTagByIdQuery>
{
    public GetTagByIdQueryValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(localizer["TagIdRequired"]);
    }
}
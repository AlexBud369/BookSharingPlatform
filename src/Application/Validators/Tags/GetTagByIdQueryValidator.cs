using FluentValidation;
using Application.Features.Tags.Queries;

namespace Application.Validators.Tags;

public class GetTagByIdQueryValidator : AbstractValidator<GetTagByIdQuery>
{
    public GetTagByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Tag ID is required");
    }
}
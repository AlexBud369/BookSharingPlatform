using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Features.Users.Queries;

namespace Application.Validators.Users;

public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(localizer["UserIdRequired"]);
    }
}
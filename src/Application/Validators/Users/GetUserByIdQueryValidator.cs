using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Common;
using Application.Features.Users.Queries;

namespace Application.Validators.Users;

public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.User.Required.UserIdRequired));
    }
}
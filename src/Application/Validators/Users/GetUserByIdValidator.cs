using Application.Common;
using Application.Features.Users.Queries;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Users;

public class GetUserByIdValidator : AbstractValidator<GetUserById.Query>
{
    public GetUserByIdValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.UserIdRequired));
    }
}
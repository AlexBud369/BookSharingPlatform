using Application.Common;
using Application.Features.Users.Commands;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Users;

public class UpdateUserValidator : AbstractValidator<UpdateUser>
{
    public UpdateUserValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.UserIdRequired));

        RuleFor(x => x.RequestingUserId)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.UserIdRequired));

        RuleFor(x => x.UserUpdateDto)
            .NotNull().WithMessage(localizer.GetString(SharedResources.UserUpdateRequired));
    }
}
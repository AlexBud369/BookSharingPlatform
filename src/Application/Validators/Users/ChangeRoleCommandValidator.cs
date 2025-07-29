using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Common;
using Application.Features.Users.Commands;

namespace Application.Validators.Users;

public class ChangeRoleCommandValidator : AbstractValidator<ChangeRoleCommand>
{
    public ChangeRoleCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.User.Required.UserIdRequired));

        RuleFor(x => x.AdminId)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.User.Required.AdminIdRequired));

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.User.Required.RoleRequired))
            .Must(role => role == DomainConstants.User.Roles.User || role == DomainConstants.User.Roles.Admin)
            .WithMessage(localizer.GetString(SharedResources.User.Validation.InvalidRole));
    }
}
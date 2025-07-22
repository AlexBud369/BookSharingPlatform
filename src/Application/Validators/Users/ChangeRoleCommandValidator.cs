using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Features.Users.Commands;

namespace Application.Validators.Users;

public class ChangeRoleCommandValidator : AbstractValidator<ChangeRoleCommand>
{
    public ChangeRoleCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(localizer["UserIdRequired"]);

        RuleFor(x => x.AdminId)
            .NotEmpty().WithMessage(localizer["AdminIdRequired"]);

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage(localizer["RoleRequired"])
            .Must(role => role == DomainConstants.User.Roles.User || role == DomainConstants.User.Roles.Admin)
            .WithMessage(localizer["InvalidRole"]);
    }
}
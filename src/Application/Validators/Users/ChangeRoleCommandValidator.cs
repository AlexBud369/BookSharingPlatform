using Application.Common;
using Application.Features.Users.Commands;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Users;

public class ChangeRoleCommandValidator : AbstractValidator<ChangeRoleCommand>
{
    public ChangeRoleCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.UserIdRequired));

        RuleFor(x => x.AdminId)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.AdminIdRequired));

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.RoleRequired))
            .Must(role => Enum.TryParse<Domain.Enums.UserRole>(role, true, out _))
            .WithMessage(localizer.GetString(SharedResources.InvalidRole, string.Join(", ", DomainConstants.User.AllowedRoles)));
    }
}
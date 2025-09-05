using Application.Common;
using Application.Features.Users.Commands;
using Domain.Constants;
using Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;

namespace Application.Validators.Users;

public class ChangeRoleValidator : AbstractValidator<ChangeRole>
{
    public ChangeRoleValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage(localizer.GetString(SharedResources.UserIdRequired));

        RuleFor(x => x.AdminId)
            .NotEqual(Guid.Empty)
            .WithMessage(localizer.GetString(SharedResources.AdminIdRequired));

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage(localizer.GetString(SharedResources.RoleRequired))
            .Must(role => DomainConstants.User.AllowedRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
            .WithMessage(localizer.GetString(SharedResources.InvalidRole, string.Join(", ", DomainConstants.User.AllowedRoles)));
    }
}
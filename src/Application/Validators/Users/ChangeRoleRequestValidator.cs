using Application.Common;
using Application.DTOs.User;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Users;

public class ChangeRoleRequestValidator : AbstractValidator<ChangeRoleRequestDto>
{
    public ChangeRoleRequestValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.RoleRequired))
            .Must(role => Enum.TryParse<Domain.Enums.UserRole>(role, true, out _))
            .WithMessage(localizer.GetString(SharedResources.InvalidRole, string.Join(", ", DomainConstants.User.AllowedRoles)));
    }
}
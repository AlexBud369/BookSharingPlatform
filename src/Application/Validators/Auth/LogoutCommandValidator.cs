using Application.Common;
using Application.Features.Auth.Commands;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Auth;

public class LogoutCommandValidator : AbstractValidator<Logout.Command>
{
    public LogoutCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.UserIdRequired));

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.RefreshTokenRequired))
            .MaximumLength(DomainConstants.User.RefreshTokenMaxLength)
            .WithMessage(localizer.GetString(SharedResources.RefreshTokenTooLong));
    }
}
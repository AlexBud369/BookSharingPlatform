using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Features.Auth.Commands;

namespace Application.Validators.Auth;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(localizer["UserIdRequired"]);

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage(localizer["RefreshTokenRequired"])
            .MaximumLength(DomainConstants.User.RefreshTokenMaxLength).WithMessage(localizer["RefreshTokenTooLong"]);
    }
}
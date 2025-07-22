using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Features.Auth.Commands;

namespace Application.Validators.Auth;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer["EmailRequired"])
            .EmailAddress().WithMessage(localizer["InvalidEmailFormat"])
            .MaximumLength(DomainConstants.User.EmailMaxLength)
            .WithMessage(localizer["EmailTooLong"]);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(localizer["PasswordRequired"])
            .MinimumLength(DomainConstants.User.PasswordMinLength)
            .WithMessage(localizer["PasswordTooShort"]);
    }
}
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Features.Auth.Commands;

namespace Application.Validators.Auth;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer["EmailRequired"])
            .EmailAddress().WithMessage(localizer["InvalidEmailFormat"])
            .MaximumLength(DomainConstants.User.EmailMaxLength)
            .WithMessage(localizer["EmailTooLong"]);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(localizer["PasswordRequired"])
            .MinimumLength(DomainConstants.User.PasswordMinLength)
            .WithMessage(localizer["PasswordTooShort"])
            .Matches(@"[A-Z]")
            .WithMessage(localizer["PasswordRequiresUppercase"])
            .Matches(@"[0-9]")
            .WithMessage(localizer["PasswordRequiresNumber"]);

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage(localizer["UsernameRequired"])
            .MaximumLength(50omainConstants.User.UsernameMaxLength)
            .WithMessage(ocalizer["UsernameTooLong"])
            .Matches(@"^[a-zA-Z0-9_]+$")
            .WithMessage(localizer["InvalidUsernameFormat"]);
    }
}
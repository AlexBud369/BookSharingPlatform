using Application.Common;
using Application.DTOs.User;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Auth;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.UsernameRequired))
            .MaximumLength(DomainConstants.User.UsernameMaxLength).WithMessage(localizer.GetString(SharedResources.UsernameTooLong))
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage(localizer.GetString(SharedResources.InvalidUsernameFormat));

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.EmailRequired))
            .EmailAddress().WithMessage(localizer.GetString(SharedResources.InvalidEmailFormat))
            .MaximumLength(DomainConstants.User.EmailMaxLength).WithMessage(localizer.GetString(SharedResources.EmailTooLong));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.PasswordRequired))
            .MinimumLength(DomainConstants.User.PasswordMinLength).WithMessage(localizer.GetString(SharedResources.PasswordTooShort))
            .Matches(@"[A-Z]").WithMessage(localizer.GetString(SharedResources.PasswordRequiresUppercase))
            .Matches(@"[0-9]").WithMessage(localizer.GetString(SharedResources.PasswordRequiresNumber));
    }
}
using Application.Common;
using Application.DTOs.User;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Auth;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.EmailRequired))
            .EmailAddress().WithMessage(localizer.GetString(SharedResources.InvalidEmailFormat))
            .MaximumLength(DomainConstants.User.EmailMaxLength).WithMessage(localizer.GetString(SharedResources.EmailTooLong));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.PasswordRequired))
            .MinimumLength(DomainConstants.User.PasswordMinLength).WithMessage(localizer.GetString(SharedResources.PasswordTooShort))
            .MaximumLength(DomainConstants.User.PasswordMaxLength).WithMessage(localizer.GetString(SharedResources.MaxLength));
    }
}
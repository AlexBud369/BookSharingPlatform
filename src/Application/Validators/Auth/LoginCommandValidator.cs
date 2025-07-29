using Application.Common;
using Application.Features.Auth.Commands;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;

namespace Application.Validators.Auth;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.EmailRequired))
            .EmailAddress().WithMessage(localizer.GetString(SharedResources.InvalidEmailFormat))
            .MaximumLength(DomainConstants.User.EmailMaxLength)
            .WithMessage(localizer.GetString(SharedResources.EmailTooLong));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.PasswordRequired))
            .MinimumLength(DomainConstants.User.PasswordMinLength)
            .WithMessage(localizer.GetString(SharedResources.PasswordTooShort));
    }
}
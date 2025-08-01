using Application.Common;
using Application.DTOs;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Auth;

public class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
{
    public RefreshTokenDtoValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.RefreshTokenRequired))
            .MaximumLength(DomainConstants.User.RefreshTokenMaxLength).WithMessage(localizer.GetString(SharedResources.RefreshTokenTooLong));
    }
}
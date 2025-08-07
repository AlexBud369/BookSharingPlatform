using Application.Common;
using Application.DTOs.User;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Users;

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Username)
            .NotEmpty().When(x => x.Username != null).WithMessage(localizer.GetString(SharedResources.EmptyString))
            .MaximumLength(DomainConstants.User.UsernameMaxLength).When(x => x.Username != null).WithMessage(localizer.GetString(SharedResources.UsernameTooLong))
            .Matches(@"^[a-zA-Z0-9]*$").When(x => x.Username != null).WithMessage(localizer.GetString(SharedResources.InvalidUsernameFormat));

        RuleFor(x => x.Email)
            .NotEmpty().When(x => x.Email != null).WithMessage(localizer.GetString(SharedResources.EmptyString))
            .MaximumLength(DomainConstants.User.EmailMaxLength).When(x => x.Email != null).WithMessage(localizer.GetString(SharedResources.EmailTooLong))
            .EmailAddress().When(x => x.Email != null).WithMessage(localizer.GetString(SharedResources.InvalidEmailFormat));

        RuleFor(x => x.Password)
            .NotEmpty().When(x => x.Password != null).WithMessage(localizer.GetString(SharedResources.EmptyString))
            .MinimumLength(DomainConstants.User.PasswordMinLength).When(x => x.Password != null).WithMessage(localizer.GetString(SharedResources.PasswordTooShort))
            .Matches(@"[A-Z]").When(x => x.Password != null).WithMessage(localizer.GetString(SharedResources.PasswordRequiresUppercase))
            .Matches(@"[0-9]").When(x => x.Password != null).WithMessage(localizer.GetString(SharedResources.PasswordRequiresNumber));

        RuleFor(x => x)
            .Must(x => x.Username != null || x.Email != null || x.Password != null)
            .WithMessage(localizer.GetString(SharedResources.AtLeastOneFieldRequired));
    }
}
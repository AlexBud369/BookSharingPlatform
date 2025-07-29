using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Common;
using Application.Features.Users.Commands;

namespace Application.Validators.Users;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.User.Required.UserIdRequired));

        RuleFor(x => x.AdminId)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.User.Required.AdminIdRequired));
    }
}
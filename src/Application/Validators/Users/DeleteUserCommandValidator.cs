using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Features.Users.Commands;

namespace Application.Validators.Users;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(localizer["UserIdRequired"]);

        RuleFor(x => x.AdminId)
            .NotEmpty().WithMessage(localizer["AdminIdRequired"]);
    }
}
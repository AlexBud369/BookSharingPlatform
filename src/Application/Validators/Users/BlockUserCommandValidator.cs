using FluentValidation;
using Application.Features.Users.Commands;

namespace Application.Validators.Users;

public class BlockUserCommandValidator : AbstractValidator<BlockUserCommand>
{
    public BlockUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.AdminId)
            .NotEmpty().WithMessage("Admin ID is required.");
    }
}
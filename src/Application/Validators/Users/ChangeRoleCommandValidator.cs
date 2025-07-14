using FluentValidation;
using Application.Features.Users.Commands;

namespace Application.Validators.Users;

public class ChangeRoleCommandValidator : AbstractValidator<ChangeRoleCommand>
{
    public ChangeRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.AdminId)
            .NotEmpty().WithMessage("Admin ID is required.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(role => role == "User" || role == "Admin")
            .WithMessage("Role must be either 'User' or 'Admin'.");
    }
}
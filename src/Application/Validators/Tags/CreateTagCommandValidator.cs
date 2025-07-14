using FluentValidation;
using Application.Features.Tags.Commands;

namespace Application.Validators.Tags;

public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(x => x.tagName)
            .NotEmpty().WithMessage("Tag name is required")
            .MaximumLength(50).WithMessage("Tag name must not exceed 50 characters");
    }
}
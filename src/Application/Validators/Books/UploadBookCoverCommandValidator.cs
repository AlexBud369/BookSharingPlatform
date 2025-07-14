using FluentValidation;
using Application.Features.Books.Commands;

namespace Application.Validators.Books;

public class UploadBookCoverCommandValidator : AbstractValidator<UploadBookCoverCommand>
{
    public UploadBookCoverCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Book ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.CoverImageUrl)
            .NotEmpty().WithMessage("Cover image URL is required")
            .MaximumLength(500).WithMessage("Cover image URL must not exceed 500 characters")
            .Matches(@"^https?://").WithMessage("Cover image URL must be a valid URL");
    }
}
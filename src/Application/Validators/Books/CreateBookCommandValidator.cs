using FluentValidation;
using Application.DTOs.Book;

namespace Application.Validators.Books;

public class CreateBookCommandValidator : AbstractValidator<BookCreateDto>
{
    public CreateBookCommandValidator()
    {
        RuleFor(b => b.UserId)
             .NotEmpty().WithMessage("User ID is required");

        RuleFor(b => b.Book)
            .NotNull().WithMessage("Book data is required");

        RuleFor(b => b.Book.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters")
            .When(b => b.Book != null);

        RuleFor(b => b.Book.Author)
            .NotEmpty().WithMessage("Author is required")
            .MaximumLength(100).WithMessage("Author must not exceed 100 characters")
            .When(b => b.Book != null);

        RuleFor(b => b.Book.Description)
            .MaximumLength(1500).WithMessage("Description must not exceed 1500 characters")
            .When(b => b.Book != null && b.Book.Description != null);

        RuleFor(b => b.Book.Tags)
            .NotNull().WithMessage("Tags cannot be null")
            .Must(tags => tags.All(tag => !string.IsNullOrEmpty(tag)))
            .WithMessage("All tags must be non-empty")
            .When(b => b.Book != null && b.Book.Tags != null);

        RuleForEach(b => b.Book.Tags)
            .MaximumLength(50).WithMessage("Each tag must not exceed 50 characters")
            .When(b => b.Book != null && b.Book.Tags != null);
    }
}

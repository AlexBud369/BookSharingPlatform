using FluentValidation;
using Application.Features.Books.Queries;

namespace Application.Validators.Books;

public class GetBookByIdQueryValidator : AbstractValidator<GetBookByIdQuery>
{
    public GetBookByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Book ID is required");
    }
}
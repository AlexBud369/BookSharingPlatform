using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Features.Books.Queries;

namespace Application.Validators.Books;

public class GetBookByIdQueryValidator : AbstractValidator<GetBookByIdQuery>
{
    public GetBookByIdQueryValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(localizer["BookIdRequired"]);
    }
}
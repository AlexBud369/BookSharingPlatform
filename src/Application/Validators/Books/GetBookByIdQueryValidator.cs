using Application.Common;
using Application.Features.Books.Queries;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;

namespace Application.Validators.Books;

public class GetBookByIdQueryValidator : AbstractValidator<GetBookById.Query>
{
    public GetBookByIdQueryValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.BookIdRequired));
    }
}
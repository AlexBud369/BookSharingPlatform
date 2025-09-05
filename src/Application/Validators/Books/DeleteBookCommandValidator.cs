using Application.Common;
using Application.Features.Books.Commands;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;

namespace Application.Validators.Books;

public class DeleteBookCommandValidator : AbstractValidator<DeleteBook.Command>
{
    public DeleteBookCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.BookIdRequired));

        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.UserIdRequired));
    }
}
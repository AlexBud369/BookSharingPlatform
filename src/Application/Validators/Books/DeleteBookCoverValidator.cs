using Application.Common;
using Application.Features.Books.Commands;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Files;

public class DeleteBookCoverValidator : AbstractValidator<DeleteBookCover.Command>
{
    public DeleteBookCoverValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage(localizer.GetString(SharedResources.FileNameRequired));

        RuleFor(x => x.BookId)
            .NotEqual(Guid.Empty)
            .WithMessage(localizer.GetString(SharedResources.BookIdRequired));

        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage(localizer.GetString(SharedResources.UserIdRequired));
    }
}
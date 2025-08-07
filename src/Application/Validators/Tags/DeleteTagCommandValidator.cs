using Application.Common;
using Application.Features.Tags.Commands;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Tags;

public class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
{
    public DeleteTagCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.TagIdRequired));
    }
}
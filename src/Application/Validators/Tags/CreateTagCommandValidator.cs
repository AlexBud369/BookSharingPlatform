using Application.Common;
using Application.Features.Tags.Commands;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Tags;

public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.TagName)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.TagNameRequired))
            .MaximumLength(DomainConstants.Tag.NameMaxLength).WithMessage(localizer.GetString(SharedResources.TagNameTooLong))
            .Matches(@"^[a-zA-Z0-9\s-]*$").WithMessage(localizer.GetString(SharedResources.InvalidTagNameFormat));
    }
}
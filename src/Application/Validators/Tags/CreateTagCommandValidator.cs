using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Common;
using Application.Features.Tags.Commands;

namespace Application.Validators.Tags;

public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.TagName)
            .NotEmpty().WithMessage(localizer.GetString(SharedResources.Tag.Required.TagNameRequired))
            .MaximumLength(DomainConstants.Tag.NameMaxLength).WithMessage(localizer.GetString(SharedResources.Tag.Length.TagNameTooLong))
            .Matches(@"^[a-zA-Z0-9\s-]*$").WithMessage(localizer.GetString(SharedResources.Tag.Validation.InvalidTagNameFormat));
    }
}
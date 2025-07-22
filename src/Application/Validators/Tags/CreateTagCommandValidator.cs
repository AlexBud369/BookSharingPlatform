using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Features.Tags.Commands;

namespace Application.Validators.Tags;

public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.TagName)
             .NotEmpty().WithMessage(localizer["TagNameRequired"])
             .MaximumLength(DomainConstants.Tag.NameMaxLength).WithMessage(localizer["TagNameTooLong"]);
    }
}
using Application.Common;
using Application.Features.Users.Commands;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Users;

public class BlockUserValidator : AbstractValidator<BlockUser>
{
    public BlockUserValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.UserIdRequired));

        RuleFor(x => x.AdminId)
            .NotEqual(Guid.Empty).WithMessage(localizer.GetString(SharedResources.AdminIdRequired));
    }
}
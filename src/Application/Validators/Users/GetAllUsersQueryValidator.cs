using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Common;
using Application.Features.Users.Queries;

namespace Application.Validators.Users;

public class GetAllUsersQueryValidator : AbstractValidator<GetAllUsersQuery>
{
    public GetAllUsersQueryValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(DomainConstants.User.DefaultPageNumber).WithMessage(localizer.GetString(SharedResources.Common.Validation.InvalidPageNumber));

        RuleFor(x => x.PageSize)
            .InclusiveBetween(DomainConstants.User.DefaultPageSize, DomainConstants.User.MaxPageSize).WithMessage(localizer.GetString(SharedResources.Common.Validation.InvalidPageSize));

        RuleFor(x => x.Email)
            .MaximumLength(DomainConstants.User.EmailMaxLength).WithMessage(localizer.GetString(SharedResources.User.Length.EmailTooLong))
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.UserName)
            .MaximumLength(DomainConstants.User.UsernameMaxLength).WithMessage(localizer.GetString(SharedResources.User.Length.UsernameTooLong))
            .When(x => !string.IsNullOrEmpty(x.UserName));
    }
}
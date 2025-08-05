using Application.Common;
using Application.Features.Users.Queries;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Application.Validators.Users;

public class GetAllUsersQueryValidator : AbstractValidator<GetAllUsersQuery>
{
    public GetAllUsersQueryValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(DomainConstants.User.DefaultPageNumber).WithMessage(localizer.GetString(SharedResources.InvalidPageNumber));

        RuleFor(x => x.PageSize)
            .InclusiveBetween(DomainConstants.User.DefaultPageSize, DomainConstants.User.MaxPageSize).WithMessage(localizer.GetString(SharedResources.InvalidPageSize));

        RuleFor(x => x.Email)
            .MaximumLength(DomainConstants.User.EmailMaxLength).WithMessage(localizer.GetString(SharedResources.EmailTooLong))
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.UserName)
            .MaximumLength(DomainConstants.User.UsernameMaxLength).WithMessage(localizer.GetString(SharedResources.UsernameTooLong))
            .When(x => !string.IsNullOrEmpty(x.UserName));
    }
}
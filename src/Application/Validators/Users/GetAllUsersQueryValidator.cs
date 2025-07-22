using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Application.Features.Users.Queries;

namespace Application.Validators.Users;

public class GetAllUsersQueryValidator : AbstractValidator<GetAllUsersQuery>
{
    public GetAllUsersQueryValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(DomainConstants.Book.DefaultPageNumber).WithMessage(localizer["InvalidPageNumber"]);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, DomainConstants.Book.MaxPageSize).WithMessage(localizer["InvalidPageSize"]);

        RuleFor(x => x.Email)
            .MaximumLength(DomainConstants.User.EmailMaxLength).WithMessage(localizer["EmailTooLong"])
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.UserName)
            .MaximumLength(DomainConstants.User.UsernameMaxLength).WithMessage(localizer["UsernameTooLong"])
            .When(x => !string.IsNullOrEmpty(x.UserName));
    }
}
using Application.Common;
using Application.DTOs;
using Application.DTOs.User;
using Application.Interfaces;
using Domain.Constants;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Application.Features.Users.Queries;

public static class GetAllUsers
{
    public class Query : IRequest<PagedResponseDto<UserDto>>
    {
        public int PageNumber { get; set; } = DomainConstants.User.DefaultPageNumber;
        public int PageSize { get; set; } = DomainConstants.User.DefaultPageSize;
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public bool? IsBlocked { get; set; }
    }

    public class Handler : IRequestHandler<Query, PagedResponseDto<UserDto>>
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IUserQueryService _userQueryService;

        public Handler(
            IStringLocalizer<SharedResources> localizer,
            IUserQueryService userQueryService)
        {
            _localizer = localizer;
            _userQueryService = userQueryService;
            Guard.Initialize(_localizer);
        }

        public async Task<PagedResponseDto<UserDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            Guard.AgainstInvalidPageNumber(request.PageNumber, nameof(request.PageNumber), _localizer.GetString(SharedResources.InvalidPageNumber));
            Guard.AgainstInvalidPageSize(request.PageSize, nameof(request.PageSize), _localizer.GetString(SharedResources.InvalidPageSize));

            return await _userQueryService.GetUsersAsync(
                request.PageNumber,
                request.PageSize,
                request.Email,
                request.UserName,
                request.IsBlocked,
                cancellationToken);
        }
    }
}
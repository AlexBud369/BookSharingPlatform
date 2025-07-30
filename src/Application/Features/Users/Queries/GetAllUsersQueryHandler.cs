using Application.Common;
using Application.DTOs.User;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Users.Queries;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly IUserQueryService _userQueryService;

    public GetAllUsersQueryHandler(
        IStringLocalizer<SharedResources> localizer,
        IUserQueryService userQueryService)
    {
        Guard.AgainstNull(localizer, nameof(localizer), localizer.GetString(SharedResources.LocalizerRequired));
        Guard.AgainstNull(userQueryService, nameof(userQueryService), localizer.GetString(SharedResources.UserQueryServiceRequired));

        _localizer = localizer;
        _userQueryService = userQueryService;
        Guard.Initialize(_localizer);
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
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
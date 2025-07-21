using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.DTOs.User;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Application.Features.Users.Queries;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly IUserQueryService _userQueryService;


    public GetAllUsersQueryHandler(
        IStringLocalizer<SharedResource> localizer,
        IUserQueryService userQueryService)
    {
        _localizer = localizer;
        _userQueryService = userQueryService;
        Guard.Initialize(_localizer);
    }
    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        return await _userQueryService.GetUsersAsync(
            request.PageNumber,
            request.PageSize,
            request.Email,
            request.UserName,
            request.IsBlocked,
            cancellationToken);
    }
}
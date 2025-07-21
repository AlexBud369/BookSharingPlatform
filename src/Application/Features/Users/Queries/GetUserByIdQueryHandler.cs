using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.DTOs.User;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Application.Features.Users.Queries;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly IUserQueryService _userQueryService;


    public GetUserByIdQueryHandler(
        IStringLocalizer<SharedResource> localizer,
        IUserQueryService userQueryService)
    {
        _localizer = localizer;
        _userQueryService = userQueryService;
        Guard.Initialize(_localizer);
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await _userQueryService.GetUserByIdAsync(request.Id, cancellationToken);
    }
}

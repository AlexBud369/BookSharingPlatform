using Application.Common;
using Application.DTOs.User;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Users.Queries;

public static class GetUserById
{
    public class Query : IRequest<UserDto>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, UserDto>
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IUserQueryService _userQueryService;

        public Handler(
            IStringLocalizer<SharedResources> localizer,
            IUserQueryService userQueryService)
        {
            Guard.AgainstNull(localizer, nameof(localizer), localizer.GetString(SharedResources.LocalizerRequired));
            Guard.AgainstNull(userQueryService, nameof(userQueryService), localizer.GetString(SharedResources.UserQueryServiceRequired));

            _localizer = localizer;
            _userQueryService = userQueryService;
            Guard.Initialize(_localizer);
        }

        public async Task<UserDto> Handle(Query request, CancellationToken cancellationToken)
        {
            Guard.AgainstEmptyGuid(request.Id, nameof(request.Id), _localizer.GetString(SharedResources.UserIdRequired));

            return await _userQueryService.GetUserByIdAsync(request.Id, cancellationToken);
        }
    }
}
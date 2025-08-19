using Application.Common;
using Application.DTOs.User;
using Application.Interfaces;
using Domain.Enums;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands;

public class UpdateUser : IRequest<UserDto>
{
    public Guid UserId { get; set; }
    public Guid RequestingUserId { get; set; }
    public UserUpdateDto UserUpdateDto { get; set; }

    public class Handler
    {
        private readonly IUserService _userService;

        public Handler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UserDto> Handle(UpdateUser request, CancellationToken cancellationToken)
        {
            var user = await _userService.UpdateUserAsync(
                request.UserId,
                request.RequestingUserId,
                request.UserUpdateDto,
                cancellationToken);

            return await _userService.MapToUserDtoAsync(user, cancellationToken);
        }
    }
}
using Application.DTOs.User;
using MediatR;
using System;

namespace Application.Features.Users.Commands;

public class UpdateUserCommand : IRequest<UserDto>
{
    public Guid UserId { get; set; }
    public Guid RequestingUserId { get; set; }
    public UserUpdateDto UserUpdateDto { get; set; }
}
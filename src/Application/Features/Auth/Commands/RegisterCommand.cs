using MediatR;
using Application.DTOs.User;

namespace Application.Features.Auth.Commands;

public class RegisterCommand : IRequest<UserDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

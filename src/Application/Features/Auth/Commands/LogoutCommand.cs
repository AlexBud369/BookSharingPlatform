using System;
using MediatR;

namespace Application.Features.Auth.Commands;

public class LogoutCommand : IRequest
{

    public Guid UserId { get; set; }

    public string RefreshToken { get; set; } = string.Empty;
}
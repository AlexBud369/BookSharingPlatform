using MediatR;

namespace Application.Features.Users.Commands;

public class ChangeRoleCommand : IRequest
{
    public Guid UserId { get; set; }
    public Guid AdminId { get; set; }
    public string Role { get; set; } = string.Empty;
}
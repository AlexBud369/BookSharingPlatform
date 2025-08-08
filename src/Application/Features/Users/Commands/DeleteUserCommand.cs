using MediatR;

namespace Application.Features.Users.Commands;

public class DeleteUserCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid AdminId { get; set; }
}
using MediatR;

namespace Application.Features.Users.Commands;

public class BlockUserCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid AdminId { get; set; }
}
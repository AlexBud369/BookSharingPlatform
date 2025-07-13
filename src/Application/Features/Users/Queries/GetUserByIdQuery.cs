using MediatR;
using Application.DTOs.User;

namespace Application.Features.Users.Queries;

public class GetUserByIdQuery : IRequest<UserDto>
{
    public Guid Id { get; set; }
}

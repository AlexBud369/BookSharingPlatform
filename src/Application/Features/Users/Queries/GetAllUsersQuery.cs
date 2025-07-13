using MediatR;
using Application.DTOs.User;

namespace Application.Features.Users.Queries;

public class GetAllUsersQuery : IRequest<IEnumerable<UserDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public bool? IsBlocked { get; set; }
}
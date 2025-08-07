using Application.DTOs.User;
using Domain.Constants;
using MediatR;

namespace Application.Features.Users.Queries;

public class GetAllUsersQuery : IRequest<IEnumerable<UserDto>>
{
    public int PageNumber { get; set; } = DomainConstants.User.DefaultPageNumber;
    public int PageSize { get; set; } = DomainConstants.User.DefaultPageSize;
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public bool? IsBlocked { get; set; }
}
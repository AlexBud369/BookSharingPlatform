using Application.DTOs;
using Application.DTOs.User;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IUserQueryService
{
    Task<PagedResponseDto<UserDto>> GetUsersAsync(
        int pageNumber,
        int pageSize,
        string? email,
        string? userName,
        bool? isBlocked,
        CancellationToken cancellationToken);

    Task<UserDto> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
}
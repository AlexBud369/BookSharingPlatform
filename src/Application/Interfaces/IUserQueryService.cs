using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.User;

namespace Application.Interfaces;

public interface IUserQueryService
{
    Task<IEnumerable<UserDto>> GetUsersAsync(
        int pageNumber,
        int pageSize, 
        string? email,
        string? userName,
        bool? isBlocked,
        CancellationToken cancellationToken);
    Task<UserDto> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
}
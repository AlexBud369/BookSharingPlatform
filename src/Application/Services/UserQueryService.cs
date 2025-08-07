using System;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.DTOs.User;
using Application.Interfaces;
using Domain.Entities;
using Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Application.Services;

public class UserQueryService : IUserQueryService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public UserQueryService(
        AppDbContext context,
        IMapper mapper,
        IStringLocalizer<SharedResources> localizer)
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync(int pageNumber, int pageSize, string? email, string? userName, bool? isBlocked, CancellationToken cancellationToken)
    {
        Guard.AgainstInvalidPageNumber(pageNumber, nameof(pageNumber), _localizer.GetString(SharedResources.InvalidPageNumber));
        Guard.AgainstInvalidPageSize(pageSize, nameof(pageSize), _localizer.GetString(SharedResources.InvalidPageSize));

        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(email)) {
            query = query.Where(u => u.Email.Contains(email));
        }
        if (!string.IsNullOrEmpty(userName)){
            query = query.Where(u => u.UserName.Contains(userName));
        }
        if (isBlocked.HasValue) {
            query = query.Where(u => u.IsBlocked == isBlocked.Value);
        }

        var users = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(userId, nameof(userId), _localizer.GetString(SharedResources.UserIdRequired));

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        Guard.AgainstNull(user, nameof(userId), _localizer.GetString(SharedResources.UserNotFound), userId.ToString());
        return _mapper.Map<UserDto>(user);
    }
}
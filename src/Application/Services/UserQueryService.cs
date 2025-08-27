using Application.Common;
using Application.DTOs;
using Application.DTOs.User;
using Application.Interfaces;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Persistence.Data;

namespace Application.Services;

public class UserQueryService : IUserQueryService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public UserQueryService(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        IStringLocalizer<SharedResources> localizer)
    {
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    public async Task<PagedResponseDto<UserDto>> GetUsersAsync(
        int pageNumber,
        int pageSize,
        string? email,
        string? userName,
        bool? isBlocked,
        CancellationToken cancellationToken)
    {
        Guard.AgainstInvalidPageNumber(pageNumber, nameof(pageNumber), _localizer.GetString(SharedResources.InvalidPageNumber));
        Guard.AgainstInvalidPageSize(pageSize, nameof(pageSize), _localizer.GetString(SharedResources.InvalidPageSize));

        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(email)) {
            query = query.Where(u => u.Email.Contains(email, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(userName)) {
            query = query.Where(u => u.UserName.Contains(userName, StringComparison.OrdinalIgnoreCase));
        }

        if (isBlocked.HasValue) {
            query = query.Where(u => u.IsBlocked == isBlocked.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var users = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var userDtos = _mapper.Map<List<UserDto>>(users);

        return new PagedResponseDto<UserDto>
        {
            Items = userDtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<UserDto> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(userId, nameof(userId), _localizer.GetString(SharedResources.UserIdRequired));

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        Guard.AgainstNull(user, nameof(userId), _localizer.GetString(SharedResources.UserNotFound), userId.ToString());

        var roles = await _userManager.GetRolesAsync(user);
        return _mapper.Map<UserDto>(user, opts => opts.Items[DomainConstants.Mapper.RoleKey] = roles.FirstOrDefault() ?? UserRole.User.ToString());
    }
}
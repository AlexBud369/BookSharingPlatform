using Application.Common;
using Application.DTOs.User;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IMapper mapper,
        IStringLocalizer<SharedResources> localizer)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    public async Task<ApplicationUser> UpdateUserAsync(
        Guid userId,
        Guid requestingUserId,
        UserUpdateDto userUpdateDto,
        CancellationToken cancellationToken)
    {
        var user = await GetUserByIdAsync(userId, cancellationToken);
        await EnsureCanUpdateAsync(userId, requestingUserId, cancellationToken);

        if (!string.IsNullOrEmpty(userUpdateDto.Username)) {
            await UpdateUsernameAsync(user, userUpdateDto.Username, cancellationToken);
        }

        if (!string.IsNullOrEmpty(userUpdateDto.Email)) {
            await UpdateEmailAsync(user, userUpdateDto.Email, cancellationToken);
        }

        if (!string.IsNullOrEmpty(userUpdateDto.Password)) {
            await UpdatePasswordAsync(user, userUpdateDto.Password, cancellationToken);
        }

        var updateResult = await _userManager.UpdateAsync(user);
        Guard.AgainstFalse(updateResult.Succeeded, nameof(user), _localizer.GetString(SharedResources.FailedToUpdateUser), string.Join(", ", updateResult.Errors.Select(e => e.Description)));

        return user;
    }

    public async Task BlockUserAsync(Guid userId, Guid adminId, CancellationToken cancellationToken)
    {
        await EnsureIsAdminAsync(adminId, cancellationToken);
        var user = await GetUserByIdAsync(userId, cancellationToken);
        user.IsBlocked = true;
        var updateResult = await _userManager.UpdateAsync(user);
        Guard.AgainstFalse(updateResult.Succeeded, nameof(user), _localizer.GetString(SharedResources.FailedToBlockUser), string.Join(", ", updateResult.Errors.Select(e => e.Description)));
    }

    public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
    {
        Guard.AgainstNull(user, nameof(user), _localizer.GetString(SharedResources.UserNotFound));
        return await _userManager.GetRolesAsync(user);
    }

    public async Task EnsureIsAdminAsync(Guid adminId, CancellationToken cancellationToken)
    {
        var admin = await _userManager.FindByIdAsync(adminId.ToString());
        Guard.AgainstNull(admin, nameof(adminId), _localizer.GetString(SharedResources.UserNotFound), adminId.ToString());

        var isAdmin = await _userManager.IsInRoleAsync(admin, UserRole.Admin.ToString());
        Guard.AgainstFalse(isAdmin, nameof(adminId), _localizer.GetString(SharedResources.AdminOnlyAccess));
    }

    public async Task<ApplicationUser> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        Guard.AgainstNull(user, nameof(userId), _localizer.GetString(SharedResources.UserNotFound), userId.ToString());
        return user;
    }

    public async Task ChangeUserRoleAsync(Guid userId, string role, CancellationToken cancellationToken)
    {
        var user = await GetUserByIdAsync(userId, cancellationToken);

        if (!await _roleManager.RoleExistsAsync(role)) {
            var newRole = new ApplicationRole { Name = role };
            var roleResult = await _roleManager.CreateAsync(newRole);
            Guard.AgainstFalse(roleResult.Succeeded, nameof(role), _localizer.GetString(SharedResources.FailedToCreateRole), role);
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        var result = await _userManager.AddToRoleAsync(user, role);
        Guard.AgainstFalse(result.Succeeded, nameof(role), _localizer.GetString(SharedResources.FailedToAssignRole), role);
    }

    public async Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await GetUserByIdAsync(userId, cancellationToken);
        var result = await _userManager.DeleteAsync(user);
        Guard.AgainstFalse(result.Succeeded, nameof(userId), _localizer.GetString(SharedResources.FailedToDeleteUser), string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task<UserDto> MapToUserDtoAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var roles = await GetUserRolesAsync(user);
        return _mapper.Map<UserDto>(user, opts => opts.Items["Role"] = roles.FirstOrDefault() ?? UserRole.User.ToString());
    }

    private async Task EnsureCanUpdateAsync(Guid userId, Guid requestingUserId, CancellationToken cancellationToken)
    {
        var requestingUser = await _userManager.FindByIdAsync(requestingUserId.ToString());
        Guard.AgainstNull(requestingUser, nameof(requestingUserId), _localizer.GetString(SharedResources.UserNotFound), requestingUserId.ToString());

        var isAdmin = await _userManager.IsInRoleAsync(requestingUser, UserRole.Admin.ToString());
        Guard.AgainstFalse(userId == requestingUserId || isAdmin, nameof(requestingUserId), _localizer.GetString(SharedResources.UnauthorizedAccess));
    }

    private async Task UpdateUsernameAsync(ApplicationUser user, string username, CancellationToken cancellationToken)
    {
        var existingUserName = await _userManager.FindByNameAsync(username);
        Guard.AgainstFalse(existingUserName == null || existingUserName.Id == user.Id, nameof(username), _localizer.GetString(SharedResources.UsernameAlreadyTaken), username);
        user.UserName = username;
    }

    private async Task UpdateEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
    {
        var existingEmail = await _userManager.FindByEmailAsync(email);
        Guard.AgainstFalse(existingEmail == null || existingEmail.Id == user.Id, nameof(email), _localizer.GetString(SharedResources.EmailAlreadyRegistered), email);
        user.Email = email;
    }

    private async Task UpdatePasswordAsync(ApplicationUser user, string password, CancellationToken cancellationToken)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, password);
        Guard.AgainstFalse(result.Succeeded, nameof(password), _localizer.GetString(SharedResources.FailedToUpdateUser), string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}
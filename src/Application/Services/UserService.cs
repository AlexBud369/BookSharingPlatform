using Application.Common;
using Application.DTOs.User;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public UserService(
        UserManager<ApplicationUser> userManager,
        IStringLocalizer<SharedResources> localizer)
    {
        _userManager = userManager;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    public async Task<ApplicationUser> UpdateUserAsync(
        Guid userId,
        Guid requestingUserId,
        UserUpdateDto userUpdateDto,
        CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(userId, nameof(userId), SharedResources.UserIdRequired);
        Guard.AgainstEmptyGuid(requestingUserId, nameof(requestingUserId), SharedResources.UserIdRequired);
        Guard.AgainstNull(userUpdateDto, nameof(userUpdateDto), SharedResources.UserDataRequired);

        var user = await _userManager.FindByIdAsync(userId.ToString());
        Guard.AgainstNull(user, nameof(userId), SharedResources.UserNotFound, userId.ToString());

        var requestingUser = await _userManager.FindByIdAsync(requestingUserId.ToString());
        Guard.AgainstNull(requestingUser, nameof(requestingUserId), SharedResources.UserNotFound, requestingUserId.ToString());

        var isAdmin = await _userManager.IsInRoleAsync(requestingUser, "Admin");
        Guard.AgainstFalse(userId == requestingUserId || isAdmin, nameof(requestingUserId), SharedResources.UnauthorizedAccess);

        if (!string.IsNullOrEmpty(userUpdateDto.Username)) {
            var existingUserName = await _userManager.FindByNameAsync(userUpdateDto.Username);
            Guard.AgainstFalse(existingUserName == null || existingUserName.Id == userId, nameof(userUpdateDto.Username), SharedResources.UsernameAlreadyTaken, userUpdateDto.Username);
            user.UserName = userUpdateDto.Username;
        }

        if (!string.IsNullOrEmpty(userUpdateDto.Email)) {
            var existingEmail = await _userManager.FindByEmailAsync(userUpdateDto.Email);
            Guard.AgainstFalse(existingEmail == null || existingEmail.Id == userId, nameof(userUpdateDto.Email), SharedResources.EmailAlreadyRegistered, userUpdateDto.Email);
            user.Email = userUpdateDto.Email;
        }

        if (!string.IsNullOrEmpty(userUpdateDto.Password)) {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, userUpdateDto.Password);
            Guard.AgainstFalse(result.Succeeded, nameof(userUpdateDto.Password), SharedResources.FailedToUpdateUser, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        var updateResult = await _userManager.UpdateAsync(user);
        Guard.AgainstFalse(updateResult.Succeeded, nameof(user), SharedResources.FailedToUpdateUser, string.Join(", ", updateResult.Errors.Select(e => e.Description)));

        return user;
    }

    public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }
}
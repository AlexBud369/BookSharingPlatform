using Application.DTOs.User;
using Domain.Entities;

namespace Application.Interfaces;

public interface IUserService
{
    Task<ApplicationUser> UpdateUserAsync(Guid userId, Guid requestingUserId, UserUpdateDto userUpdateDto, CancellationToken cancellationToken);
    Task BlockUserAsync(Guid userId, Guid adminId, CancellationToken cancellationToken);
    Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
    Task EnsureIsAdminAsync(Guid adminId, CancellationToken cancellationToken);
    Task<ApplicationUser> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task ChangeUserRoleAsync(Guid userId, string role, CancellationToken cancellationToken);
    Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<UserDto> MapToUserDtoAsync(ApplicationUser user, CancellationToken cancellationToken);
}
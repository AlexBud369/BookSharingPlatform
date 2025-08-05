using Application.DTOs.User;
using Domain.Entities;

namespace Application.Interfaces;

public interface IUserService
{
    Task<ApplicationUser> UpdateUserAsync(
        Guid userId,
        Guid requestingUserId,
        UserUpdateDto userUpdateDto,
        CancellationToken cancellationToken);

    Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
}
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class ApplicationRole : IdentityRole<Guid>
{

    public ApplicationRole(string roleName) : base(roleName) { }

    private ApplicationRole() { }
}
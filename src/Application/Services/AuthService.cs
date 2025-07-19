using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration) {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<string> GenerateJwtTokenAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var claims = await CreateUserClaims(user);
        var creds = CreateJwtCredentials();
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Task<RefreshToken> GenerateRefreshTokenAsync(string userId, CancellationToken cancellationToken)
    {
        return Task.FromResult(new RefreshToken {
            Token = Guid.NewGuid().ToString(),
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        });
    }

    public async Task CreateUserAsync(ApplicationUser user, string password, string role, CancellationToken cancellationToken) 
    {
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded) {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new ApplicationException($"Registration failed: {errors}");
        }
        await _userManager.AddToRoleAsync(user, role);
    }

    private async Task<List<Claim>> CreateUserClaims(ApplicationUser user) {
        var claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return claims;
    }

    private SigningCredentials CreateJwtCredentials() {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

}

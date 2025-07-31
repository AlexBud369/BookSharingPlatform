using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        IStringLocalizer<SharedResources> localizer)
    {
        _userManager = userManager;
        _configuration = configuration;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    public async Task<string> GenerateJwtTokenAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(user, nameof(user), _localizer.GetString(SharedResources.UserNotFound));
        Guard.AgainstEmptyGuid(user.Id, nameof(user.Id), _localizer.GetString(SharedResources.UserIdRequired));

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var key = _configuration["Jwt:Key"];
        Guard.AgainstEmptyString(issuer, nameof(issuer), _localizer.GetString(SharedResources.JwtIssuerRequired));
        Guard.AgainstEmptyString(audience, nameof(audience), _localizer.GetString(SharedResources.JwtAudienceRequired));
        Guard.AgainstEmptyString(key, nameof(key), _localizer.GetString(SharedResources.JwtKeyRequired));

        var claims = await CreateUserClaims(user);
        var creds = CreateJwtCredentials();
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyString(userId, nameof(userId), _localizer.GetString(SharedResources.UserIdRequired));
       
        return Task.FromResult(new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        });
    }

    public async Task CreateUserAsync(ApplicationUser user, string password, string role, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(user, nameof(user), _localizer.GetString(SharedResources.UserNotFound));
        Guard.AgainstEmptyGuid(user.Id, nameof(user.Id), _localizer.GetString(SharedResources.UserIdRequired));
        Guard.AgainstEmptyString(password, nameof(password), _localizer.GetString(SharedResources.PasswordRequired));
        Guard.AgainstEmptyString(role, nameof(role), _localizer.GetString(SharedResources.RoleRequired));

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded) {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            Guard.AgainstFalse(true, nameof(user), _localizer.GetString(SharedResources.RegistrationFailed), errors);
        }
        await _userManager.AddToRoleAsync(user, role);
    }

    private async Task<List<Claim>> CreateUserClaims(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return claims;
    }

    private SigningCredentials CreateJwtCredentials()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }
}
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public JwtTokenService(
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

        var (issuer, audience, key) = ValidateJwtConfiguration();
        var claims = await CreateUserClaims(user);
        var creds = CreateJwtCredentials(key);
        var token = CreateJwtToken(issuer, audience, claims, creds);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        Guard.AgainstNull(tokenString, nameof(tokenString), _localizer.GetString(SharedResources.JwtTokenGenerationFailed));
        return tokenString;
    }

    private (string Issuer, string Audience, string Key) ValidateJwtConfiguration()
    {
        var issuer = _configuration[DomainConstants.Jwt.JwtIssuerKey];
        var audience = _configuration[DomainConstants.Jwt.JwtAudienceKey];
        var key = _configuration[DomainConstants.Jwt.JwtKey];
        Guard.AgainstEmptyString(issuer, nameof(issuer), _localizer.GetString(SharedResources.JwtIssuerRequired));
        Guard.AgainstEmptyString(audience, nameof(audience), _localizer.GetString(SharedResources.JwtAudienceRequired));
        Guard.AgainstEmptyString(key, nameof(key), _localizer.GetString(SharedResources.JwtKeyRequired));
        return (issuer, audience, key);
    }

    private async Task<List<Claim>> CreateUserClaims(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(DomainConstants.Jwt.ClaimSub, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return claims;
    }

    private SigningCredentials CreateJwtCredentials(string key)
    {
        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        return new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256 );
    }

    private JwtSecurityToken CreateJwtToken(string issuer, string audience, List<Claim> claims, SigningCredentials creds)
    {
        return new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(DomainConstants.Jwt.JwtTokenLifetimeHours),
            signingCredentials: creds);
    }
}
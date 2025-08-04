using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.DTOs;
using Application.DTOs.User;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        IStringLocalizer<SharedResources> localizer,
        IMapper mapper,
        AppDbContext context)
    {
        _userManager = userManager;
        _configuration = configuration;
        _localizer = localizer;
        _mapper = mapper;
        _context = context;
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
        Guard.AgainstEmptyGuid(userId, nameof(userId), _localizer.GetString(SharedResources.UserIdRequired));

        return Task.FromResult(new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        });
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyString(refreshTokenDto.Token, nameof(refreshTokenDto.Token), _localizer.GetString(SharedResources.RefreshTokenRequired));

        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.Token && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow, cancellationToken);

        Guard.AgainstNull(refreshToken, nameof(refreshTokenDto.Token), _localizer.GetString(SharedResources.RefreshTokenNotFound));

        Guard.AgainstUnauthorized(!refreshToken.User.IsBlocked, _localizer.GetString(SharedResources.UserBlocked));

        refreshToken.IsRevoked = true;
        _context.RefreshTokens.Update(refreshToken);

        var newRefreshToken = await GenerateRefreshTokenAsync(refreshToken.UserId, cancellationToken);
        _context.RefreshTokens.Add(newRefreshToken);

        var newJwtToken = await GenerateJwtTokenAsync(refreshToken.User, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        var roles = await _userManager.GetRolesAsync(refreshToken.User);
        var userDto = _mapper.Map<UserDto>(refreshToken.User, opts => opts.Items["Role"] = roles.FirstOrDefault() ?? UserRole.User.ToString());

        return new AuthResponseDto
        {
            AccessToken = newJwtToken,
            RefreshToken = newRefreshToken.Token,
            User = userDto
        };
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken, Guid userId, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyString(refreshToken, nameof(refreshToken), _localizer.GetString(SharedResources.RefreshTokenRequired));
        Guard.AgainstEmptyGuid(userId, nameof(userId), _localizer.GetString(SharedResources.UserIdRequired));

        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId && !rt.IsRevoked, cancellationToken);

        Guard.AgainstNull(token, nameof(refreshToken), _localizer.GetString(SharedResources.RefreshTokenNotFound));

        token.IsRevoked = true;
        _context.RefreshTokens.Update(token);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateUserAsync(ApplicationUser user, string password, string role, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(user, nameof(user), _localizer.GetString(SharedResources.UserNotFound));
        Guard.AgainstEmptyGuid(user.Id, nameof(user.Id), _localizer.GetString(SharedResources.UserIdRequired));
        Guard.AgainstEmptyString(password, nameof(password), _localizer.GetString(SharedResources.PasswordRequired));
        Guard.AgainstEmptyString(role, nameof(role), _localizer.GetString(SharedResources.RoleRequired));

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
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
using Application.Common;
using Application.DTOs;
using Application.DTOs.User;
using Application.Interfaces;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IStringLocalizer<SharedResources> localizer,
        IMapper mapper,
        AppDbContext context,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _localizer = localizer;
        _mapper = mapper;
        _context = context;
        _jwtTokenService = jwtTokenService;
        Guard.Initialize(_localizer);
    }

    public async Task<string> GenerateJwtTokenAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(user, nameof(user), _localizer.GetString(SharedResources.UserNotFound));
        Guard.AgainstEmptyGuid(user.Id, nameof(user.Id), _localizer.GetString(SharedResources.UserIdRequired));

        var token = await _jwtTokenService.GenerateJwtTokenAsync(user, cancellationToken);
        Guard.AgainstNull(token, nameof(token), _localizer.GetString(SharedResources.JwtTokenGenerationFailed));

        return token;
    }

    public Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(userId, nameof(userId), _localizer.GetString(SharedResources.UserIdRequired));

        return Task.FromResult(new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(DomainConstants.Jwt.JwtRefreshTokenLifetimeDays),
            IsRevoked = false
        });
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken)
    {
        var refreshToken = await ValidateRefreshTokenAsync(refreshTokenDto.Token, cancellationToken);
        await RevokeRefreshTokenAsync(refreshToken, cancellationToken);
        var (newJwtToken, newRefreshToken) = await GenerateNewTokensAsync(refreshToken.UserId, cancellationToken);
        return await MapToAuthResponseAsync(refreshToken.User, newJwtToken, newRefreshToken, cancellationToken);
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

    public async Task CreateUserAsync(ApplicationUser user, string password, UserRole role, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(user, nameof(user), _localizer.GetString(SharedResources.UserNotFound));
        Guard.AgainstEmptyGuid(user.Id, nameof(user.Id), _localizer.GetString(SharedResources.UserIdRequired));
        Guard.AgainstEmptyString(password, nameof(password), _localizer.GetString(SharedResources.PasswordRequired));

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded) {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            Guard.AgainstFalse(true, nameof(user), _localizer.GetString(SharedResources.RegistrationFailed), errors);
        }
        await _userManager.AddToRoleAsync(user, role.ToString());
    }

    private async Task<RefreshToken> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyString(token, nameof(token), _localizer.GetString(SharedResources.RefreshTokenRequired));

        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow, cancellationToken);

        Guard.AgainstNull(refreshToken, nameof(token), _localizer.GetString(SharedResources.RefreshTokenNotFound));
        Guard.AgainstUnauthorized(!refreshToken.User.IsBlocked, _localizer.GetString(SharedResources.UserBlocked));

        return refreshToken;
    }

    private async Task RevokeRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        refreshToken.IsRevoked = true;
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<(string JwtToken, RefreshToken RefreshToken)> GenerateNewTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        var newRefreshToken = await GenerateRefreshTokenAsync(userId, cancellationToken);
        _context.RefreshTokens.Add(newRefreshToken);
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        Guard.AgainstNull(user, nameof(user), _localizer.GetString(SharedResources.UserNotFound));
        var newJwtToken = await GenerateJwtTokenAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return (newJwtToken, newRefreshToken);
    }

    private async Task<AuthResponseDto> MapToAuthResponseAsync(ApplicationUser user, string jwtToken, RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var userDto = _mapper.Map<UserDto>(user, opts => opts.Items[DomainConstants.Mapper.RoleKey] = roles.FirstOrDefault() ?? UserRole.User.ToString());

        return new AuthResponseDto
        {
            AccessToken = jwtToken,
            RefreshToken = refreshToken.Token,
            User = userDto
        };
    }
}
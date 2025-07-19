using Application.Common;
using Application.DTOs.Auth;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly AppDbContext _context;

    public LoginCommandHandler(UserManager<ApplicationUser> userManager,
                               IMapper mapper,
                               IConfiguration configuration,
                               IStringLocalizer<SharedResource> localizer,
                               AppDbContext context)
    {
        _userManager = userManager;
        _mapper = mapper;
        _configuration = configuration;
        _localizer = localizer;
        _context = context;
        Guard.Initialize(_localizer);
    }

    /// <summary>
    /// Handles user login command, verifies email and password,
    /// creates JWT token and refresh token,
    /// returns user data and tokens.
    /// </summary>
    /// <param name="request">Command containing user email and password</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns><see cref="AuthResponseDto"/> object containing JWT token, refresh token and user data</returns>
    /// <exception cref="ApplicationException">
    /// Thrown when email/password is empty, user not found, blocked, or password is invalid
    /// </exception>
    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyString(request.Email, nameof(request.Email), "EmptyString", nameof(request.Email));
        Guard.AgainstEmptyString(request.Password, nameof(request.Password), "EmptyString", nameof(request.Password));

        var user = await _userManager.FindByEmailAsync(request.Email);
        Guard.AgainstNull(user, nameof(request.Email), "UserNotFound", request.Email);
        Guard.AgainstUnauthorized(!user.IsBlocked, "UserBlocked");

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        Guard.Against(!passwordValid, nameof(request.Password), "InvalidPassword");

        var tokenString = await _authService.GenerateJwtTokenAsync(user, cancellationToken);
        var refreshToken = await _authService.GenerateRefreshTokenAsync(user.Id, cancellationToken);
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        var roles = await _userManager.GetRolesAsync(user);
        var userDto = _mapper.Map<UserDto>(user, opts => opts.Items["Role"] = roles.FirstOrDefault() ?? "User");

        return new AuthResponseDto
        {
            AccessToken = tokenString,
            RefreshToken = refreshToken.Token,
            User = userDto
        };
    }
}
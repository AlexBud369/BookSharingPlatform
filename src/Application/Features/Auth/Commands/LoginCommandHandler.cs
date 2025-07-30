using Application.Common;
using Application.DTOs.Auth;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using System;

namespace Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        IConfiguration configuration,
        IStringLocalizer<SharedResources> localizer,
        AppDbContext context,
        IAuthService authService)
    {
        Guard.AgainstNull(userManager, nameof(userManager), localizer.GetString(SharedResources.UserManagerRequired));
        Guard.AgainstNull(mapper, nameof(mapper), localizer.GetString(SharedResources.MapperRequired));
        Guard.AgainstNull(configuration, nameof(configuration), localizer.GetString(SharedResources.ConfigurationRequired));
        Guard.AgainstNull(localizer, nameof(localizer), localizer.GetString(SharedResources.LocalizerRequired));
        Guard.AgainstNull(context, nameof(context), localizer.GetString(SharedResources.DbContextRequired));
        Guard.AgainstNull(authService, nameof(authService), localizer.GetString(SharedResources.AuthServiceRequired));

        _userManager = userManager;
        _mapper = mapper;
        _configuration = configuration;
        _localizer = localizer;
        _context = context;
        _authService = authService;
        Guard.Initialize(_localizer);
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyString(request.Email, nameof(request.Email), _localizer.GetString(SharedResources.EmptyString));
        Guard.AgainstEmptyString(request.Password, nameof(request.Password), _localizer.GetString(SharedResources.EmptyString));

        var user = await _userManager.FindByEmailAsync(request.Email);
        Guard.AgainstNull(user, nameof(request.Email), _localizer.GetString(SharedResources.UserNotFound), request.Email);
        Guard.AgainstUnauthorized(!user.IsBlocked, _localizer.GetString(SharedResources.UserBlocked));

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        Guard.Against(!passwordValid, nameof(request.Password), _localizer.GetString(SharedResources.InvalidPassword));

        var tokenString = await _authService.GenerateJwtTokenAsync(user, cancellationToken);
        var refreshToken = await _authService.GenerateRefreshTokenAsync(user.Id, cancellationToken);
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        var roles = await _userManager.GetRolesAsync(user);
        var userDto = _mapper.Map<UserDto>(user, opts => opts.Items["Role"] = roles.FirstOrDefault() ?? UserRole.User.ToString());

        return new AuthResponseDto
        {
            AccessToken = tokenString,
            RefreshToken = refreshToken.Token,
            User = userDto
        };
    }
}
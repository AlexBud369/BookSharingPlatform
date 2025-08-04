using Application.Common;
using Application.DTOs;
using Application.DTOs.User;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;

namespace Application.Features.Auth.Commands;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        IStringLocalizer<SharedResources> localizer,
        IAuthService authService)
    {
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
        _localizer = localizer;
        _authService = authService;
        Guard.Initialize(_localizer);
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyString(request.RefreshToken, nameof(request.RefreshToken), SharedResources.RefreshTokenRequired);

        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow, cancellationToken);

        Guard.AgainstNull(refreshToken, nameof(request.RefreshToken), SharedResources.RefreshTokenNotFound);

        Guard.AgainstUnauthorized(!refreshToken.User.IsBlocked, SharedResources.UserBlocked);

        refreshToken.IsRevoked = true;
        _context.RefreshTokens.Update(refreshToken);

        var newRefreshToken = await _authService.GenerateRefreshTokenAsync(refreshToken.UserId, cancellationToken);
        _context.RefreshTokens.Add(newRefreshToken);

        var newJwtToken = await _authService.GenerateJwtTokenAsync(refreshToken.User, cancellationToken);

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
}
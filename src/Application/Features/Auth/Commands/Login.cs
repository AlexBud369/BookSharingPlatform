using Application.Common;
using Application.DTOs;
using Application.DTOs.User;
using Application.Interfaces;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Persistence.Data;

namespace Application.Features.Auth.Commands;

public static class Login
{
    public class Command : IRequest<AuthResponseDto>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class Handler : IRequestHandler<Command, AuthResponseDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;

        public Handler(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IConfiguration configuration,
            IStringLocalizer<SharedResources> localizer,
            AppDbContext context,
            IAuthService authService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
            _localizer = localizer;
            _context = context;
            _authService = authService;
            Guard.Initialize(_localizer);
        }

        public async Task<AuthResponseDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await FindUserAsync(request.Email, cancellationToken);
            await ValidatePasswordAsync(user, request.Password, cancellationToken);
            var (accessToken, refreshToken) = await GenerateTokensAsync(user, cancellationToken);
            return await CreateResponseAsync(user, accessToken, refreshToken, cancellationToken);
        }

        private async Task<ApplicationUser> FindUserAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);
            Guard.AgainstNull(user, nameof(email), _localizer.GetString(SharedResources.UserNotFound), email);
            Guard.AgainstUnauthorized(!user.IsBlocked, _localizer.GetString(SharedResources.UserBlocked));
            return user;
        }

        private async Task ValidatePasswordAsync(ApplicationUser user, string password, CancellationToken cancellationToken)
        {
            var passwordValid = await _userManager.CheckPasswordAsync(user, password);
            Guard.AgainstFalse(passwordValid, nameof(password), _localizer.GetString(SharedResources.InvalidPassword));
        }

        private async Task<(string AccessToken, RefreshToken RefreshToken)> GenerateTokensAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var accessToken = await _authService.GenerateJwtTokenAsync(user, cancellationToken);
            var refreshToken = await _authService.GenerateRefreshTokenAsync(user.Id, cancellationToken);
            Guard.AgainstNull(refreshToken, nameof(refreshToken), _localizer.GetString(SharedResources.RefreshTokenNotFound));
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync(cancellationToken);
            return (accessToken, refreshToken);
        }

        private async Task<AuthResponseDto> CreateResponseAsync(ApplicationUser user, string accessToken, Domain.Entities.RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? UserRole.User.ToString();
            var userDto = _mapper.Map<UserDto>(user, opts => opts.Items[DomainConstants.Mapper.RoleKey] = role);
            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                User = userDto
            };
        }
    }
}
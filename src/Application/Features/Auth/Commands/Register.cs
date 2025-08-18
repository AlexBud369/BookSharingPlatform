using Application.Common;
using Application.DTOs.User;
using Application.Interfaces;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Application.Features.Auth.Commands;

public static class Register
{
    public class Command : IRequest<UserDto>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }

    public class Handler : IRequestHandler<Command, UserDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IAuthService _authService;

        public Handler(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IStringLocalizer<SharedResources> localizer,
            IAuthService authService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _localizer = localizer;
            _authService = authService;
            Guard.Initialize(_localizer);
        }

        public async Task<UserDto> Handle(Command request, CancellationToken cancellationToken)
        {
            await CheckExistingUserAsync(request.Email, request.UserName, cancellationToken);
            var user = await CreateUserAsync(request, cancellationToken);
            return MapToUserDto(user);
        }

        private async Task CheckExistingUserAsync(string email, string username, CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            Guard.AgainstFalse(existingUser == null, nameof(email), _localizer.GetString(SharedResources.EmailAlreadyRegistered), email);

            var existingUserName = await _userManager.FindByNameAsync(username);
            Guard.AgainstFalse(existingUserName == null, nameof(username), _localizer.GetString(SharedResources.UsernameAlreadyTaken), username);
        }

        private async Task<ApplicationUser> CreateUserAsync(Command request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.UserName
            };
            await _authService.CreateUserAsync(user, request.Password, UserRole.User, cancellationToken);
            return user;
        }

        private UserDto MapToUserDto(ApplicationUser user)
        {
            return _mapper.Map<UserDto>(user, opts => opts.Items[DomainConstants.Mapper.RoleKey] = UserRole.User.ToString());
        }
    }
}
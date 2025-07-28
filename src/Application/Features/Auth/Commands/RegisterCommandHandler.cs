using Application.Common;
using Application.DTOs.User;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System;

namespace Application.Features.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly IAuthService _authService;

    public RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        IStringLocalizer<SharedResources> localizer,
        IAuthService authService)
    {
        Guard.AgainstNull(userManager, nameof(userManager), localizer.GetString(SharedResources.UserManagerRequired));
        Guard.AgainstNull(mapper, nameof(mapper), localizer.GetString(SharedResources.MapperRequired));
        Guard.AgainstNull(localizer, nameof(localizer), localizer.GetString(SharedResources.LocalizerRequired));
        Guard.AgainstNull(authService, nameof(authService), localizer.GetString(SharedResources.AuthServiceRequired));

        _userManager = userManager;
        _mapper = mapper;
        _localizer = localizer;
        _authService = authService;
        Guard.Initialize(_localizer);
    }

    public async Task<UserDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyString(request.Email, nameof(request.Email), _localizer.GetString(SharedResources.EmptyString));
        Guard.AgainstEmptyString(request.Password, nameof(request.Password), _localizer.GetString(SharedResources.EmptyString));
        Guard.AgainstEmptyString(request.UserName, nameof(request.UserName), _localizer.GetString(SharedResources.EmptyString));

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        Guard.AgainstNull(
            existingUser == null,
            nameof(request.Email),
            _localizer.GetString(SharedResources.EmailAlreadyRegistered), 
            request.Email);

        var existingUserName = await _userManager.FindByNameAsync(request.UserName);
        Guard.AgainstNull(
            existingUserName == null,
            nameof(request.UserName),
            _localizer.GetString(SharedResources.UsernameAlreadyTaken),
            request.UserName);

        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.UserName
        };

        await _authService.CreateUserAsync(
            user,
            request.Password,
            UserRole.User.ToString(),
            cancellationToken);

        return _mapper.Map<UserDto>(user, opts => opts.Items["Role"] = UserRole.User.ToString());
    }
}
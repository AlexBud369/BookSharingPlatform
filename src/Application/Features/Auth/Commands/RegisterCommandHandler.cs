using Application.Common;
using Application.DTOs.User;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly IAuthService _authService;

    public RegisterCommand(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        IStringLocalizer<SharedResource> localizer,
        IAuthService authService)
    {
        _userManager = userManager;
        _mapper = mapper;
        _localizer = localizer;
        _authService = authService;
        Guard.Initialize(_localizer);
    }

    public async Task<UserDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyString(request.Email, nameof(request.Email), "EmptyString", nameof(request.Email));
        Guard.AgainstEmptyString(request.Password, nameof(request.Password), "EmptyString", nameof(request.Password));
        Guard.AgainstEmptyString(request.UserName, nameof(request.UserName), "EmptyString", nameof(request.UserName));

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        Guard.AgainstNull(existingUser == null, nameof(request.Email), "EmailAlreadyRegistered", request.Email);

        var existingUserName = await _userManager.FindByNameAsync(request.UserName);
        Guard.AgainstNull(existingUserName == null, nameof(request.UserName), "UsernameAlreadyTaken", request.UserName);

        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.UserName
        };

        await _authService.CreateUserAsync(user, request.Password, "User", cancellationToken);
        return _mapper.Map<UserDto>(user, opts => opts.Items["Role"] = "User");
    }
}

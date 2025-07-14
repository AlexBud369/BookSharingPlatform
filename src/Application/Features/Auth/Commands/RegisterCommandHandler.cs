using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public RegisterCommand(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email is already registered");
        }

        var existingUserName = await _userManager.FindByNameAsync(request.UserName);
        if (existingUserName != null)
        {
            throw new InvalidOperationException("Username is already taken");
        }

        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.UserName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new UnauthorizedAccessException($"Registration failed: {errors}");
        }

        await _userManager.AddToRoleAsync(user, "User");
        return _mapper.Map<UserDto>(user);
    }
}

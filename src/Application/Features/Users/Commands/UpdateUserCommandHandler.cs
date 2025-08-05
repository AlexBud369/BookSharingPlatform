using Application.DTOs.User;
using Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Application.Features.Users.Commands;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(
        IUserService userService,
        IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.UpdateUserAsync(
            request.UserId,
            request.RequestingUserId,
            request.UserUpdateDto,
            cancellationToken);

        var roles = await _userService.GetUserRolesAsync(user);
        return _mapper.Map<UserDto>(user, opts => opts.Items["Role"] = roles.FirstOrDefault() ?? "User");
    }
}
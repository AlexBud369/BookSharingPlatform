using Application.Common;
using Application.DTOs.User;
using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly IValidator<UserUpdateDto> _updateValidator;
    private readonly IValidator<ChangeRoleRequestDto> _changeRoleValidator;
    private readonly IValidator<GetAllUsersQuery> _getAllUsersValidator;
    private readonly IValidator<GetUserByIdQuery> _getUserByIdValidator;
    private readonly IValidator<DeleteUserCommand> _deleteUserValidator;
    private readonly IValidator<BlockUserCommand> _blockUserValidator;
    private readonly IValidator<ChangeRoleCommand> _changeRoleCommandValidator;

    public UsersController(
        IMediator mediator,
        IStringLocalizer<SharedResources> localizer,
        IValidator<UserUpdateDto> updateValidator,
        IValidator<ChangeRoleRequestDto> changeRoleValidator,
        IValidator<GetAllUsersQuery> getAllUsersValidator,
        IValidator<GetUserByIdQuery> getUserByIdValidator,
        IValidator<DeleteUserCommand> deleteUserValidator,
        IValidator<BlockUserCommand> blockUserValidator,
        IValidator<ChangeRoleCommand> changeRoleCommandValidator)
    {
        _mediator = mediator;
        _localizer = localizer;
        _updateValidator = updateValidator;
        _changeRoleValidator = changeRoleValidator;
        _getAllUsersValidator = getAllUsersValidator;
        _getUserByIdValidator = getUserByIdValidator;
        _deleteUserValidator = deleteUserValidator;
        _blockUserValidator = blockUserValidator;
        _changeRoleCommandValidator = changeRoleCommandValidator;
        Guard.Initialize(_localizer);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? email = null,
        [FromQuery] string? userName = null,
        [FromQuery] bool? isBlocked = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllUsersQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Email = email,
            UserName = userName,
            IsBlocked = isBlocked
        };

        var validationResult = await _getAllUsersValidator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid) {
            return BadRequest(validationResult.Errors);
        }

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUser(Guid id, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(id, nameof(id), _localizer.GetString(SharedResources.UserIdRequired));
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new ApplicationException(_localizer.GetString(SharedResources.UnauthorizedAccess)));
        var isAdmin = User.IsInRole("Admin");

        if (id != userId && !isAdmin) {
            return Forbid();
        }

        var query = new GetUserByIdQuery { Id = id };
        var validationResult = await _getUserByIdValidator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid) {
            return BadRequest(validationResult.Errors);
        }

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new ApplicationException(_localizer.GetString(SharedResources.UnauthorizedAccess)));
        var query = new GetUserByIdQuery { Id = userId };
        var validationResult = await _getUserByIdValidator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid) {
            return BadRequest(validationResult.Errors);
        }

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateDto userUpdateDto, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(id, nameof(id), _localizer.GetString(SharedResources.UserIdRequired));
        var validationResult = await _updateValidator.ValidateAsync(userUpdateDto, cancellationToken);
        if (!validationResult.IsValid) {
            return BadRequest(validationResult.Errors);
        }

        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new ApplicationException(_localizer.GetString(SharedResources.UnauthorizedAccess)));
        var command = new UpdateUserCommand
        {
            UserId = id,
            RequestingUserId = userId,
            UserUpdateDto = userUpdateDto
        };

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(id, nameof(id), _localizer.GetString(SharedResources.UserIdRequired));
        var adminId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new ApplicationException(_localizer.GetString(SharedResources.UnauthorizedAccess)));
        var command = new DeleteUserCommand { UserId = id, AdminId = adminId };
        var validationResult = await _deleteUserValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid) {
            return BadRequest(validationResult.Errors);
        }

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id}/block")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BlockUser(Guid id, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(id, nameof(id), _localizer.GetString(SharedResources.UserIdRequired));
        var adminId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new ApplicationException(_localizer.GetString(SharedResources.UnauthorizedAccess)));
        var command = new BlockUserCommand { UserId = id, AdminId = adminId };
        var validationResult = await _blockUserValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid) {
            return BadRequest(validationResult.Errors);
        }

        await _mediator.Send(command, cancellationToken);

        return Ok();
    }

    [HttpPost("{id}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeRole(
        Guid id,
        [FromBody] ChangeRoleRequestDto roleRequest,
        CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(id, nameof(id), _localizer.GetString(SharedResources.UserIdRequired));
        var validationResult = await _changeRoleValidator.ValidateAsync(roleRequest, cancellationToken);
        if (!validationResult.IsValid) {
            return BadRequest(validationResult.Errors);
        }

        var adminId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new ApplicationException(_localizer.GetString(SharedResources.UnauthorizedAccess)));
        var command = new ChangeRoleCommand { UserId = id, AdminId = adminId, Role = roleRequest.Role };
        var commandValidationResult = await _changeRoleCommandValidator.ValidateAsync(command, cancellationToken);
        if (!commandValidationResult.IsValid) {
            return BadRequest(commandValidationResult.Errors);
        }

        await _mediator.Send(command, cancellationToken);

        return Ok();
    }
}
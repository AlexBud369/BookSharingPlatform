using Application.Common;
using Application.DTOs.User;
using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using Domain.Constants;
using Domain.Enums;
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
    private readonly IValidator<UpdateUser> _updateUserValidator;
    private readonly IValidator<GetAllUsers.Query> _getAllUsersValidator;
    private readonly IValidator<GetUserById.Query> _getUserByIdValidator;
    private readonly IValidator<DeleteUser> _deleteUserValidator;
    private readonly IValidator<BlockUser.Command> _blockUserValidator;
    private readonly IValidator<ChangeRole> _changeRoleValidator;

    public UsersController(
        IMediator mediator,
        IStringLocalizer<SharedResources> localizer,
        IValidator<UserUpdateDto> updateValidator,
        IValidator<UpdateUser> updateUserValidator,
        IValidator<GetAllUsers.Query> getAllUsersValidator,
        IValidator<GetUserById.Query> getUserByIdValidator,
        IValidator<DeleteUser> deleteUserValidator,
        IValidator<BlockUser.Command> blockUserValidator,
        IValidator<ChangeRole> changeRoleValidator)
    {
        _mediator = mediator;
        _localizer = localizer;
        _updateValidator = updateValidator;
        _updateUserValidator = updateUserValidator;
        _getAllUsersValidator = getAllUsersValidator;
        _getUserByIdValidator = getUserByIdValidator;
        _deleteUserValidator = deleteUserValidator;
        _blockUserValidator = blockUserValidator;
        _changeRoleValidator = changeRoleValidator;
        Guard.Initialize(_localizer);
    }

    [HttpGet]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int pageNumber = DomainConstants.User.DefaultPageNumber,
        [FromQuery] int pageSize = DomainConstants.User.DefaultPageSize,
        [FromQuery] string? email = null,
        [FromQuery] string? userName = null,
        [FromQuery] bool? isBlocked = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllUsers.Query
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
        var userId = GetCurrentUserId();
        var isAdmin = User.IsInRole(UserRole.Admin.ToString());

        if (id != userId && !isAdmin) {
            return Forbid();
        }

        var query = new GetUserById.Query { Id = id };
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
        var userId = GetCurrentUserId();
        var query = new GetUserById.Query { Id = userId };
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

        var command = await ValidateAndCreateCommand(
            new UpdateUser { UserId = id, RequestingUserId = GetCurrentUserId(), UserUpdateDto = userUpdateDto },
            _updateUserValidator,
            cancellationToken);

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(id, nameof(id), _localizer.GetString(SharedResources.UserIdRequired));
        var command = await ValidateAndCreateCommand(
            new DeleteUser { UserId = id, AdminId = GetCurrentUserId() },
            _deleteUserValidator,
            cancellationToken);

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id}/block")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> BlockUser(Guid id, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(id, nameof(id), _localizer.GetString(SharedResources.UserIdRequired));
        var command = await ValidateAndCreateCommand(
            new BlockUser.Command { UserId = id, AdminId = GetCurrentUserId() },
            _blockUserValidator,
            cancellationToken);

        await _mediator.Send(command, cancellationToken);

        return Ok();
    }

    [HttpPost("{id}/role")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> ChangeRole(
        Guid id,
        [FromBody] ChangeRoleRequestDto roleRequest,
        CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(id, nameof(id), _localizer.GetString(SharedResources.UserIdRequired));
        var command = await ValidateAndCreateCommand(
            new ChangeRole { UserId = id, AdminId = GetCurrentUserId(), Role = roleRequest.Role },
            _changeRoleValidator,
            cancellationToken);

        await _mediator.Send(command, cancellationToken);

        return Ok();
    }

    private async Task<T> ValidateAndCreateCommand<T>(T command, IValidator<T> validator, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid) {
            throw new ValidationException(validationResult.Errors);
        }

        return command;
    }

    private Guid GetCurrentUserId()
    {
        var userIdString = User.FindFirst(DomainConstants.Jwt.ClaimSub)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId)) {
            throw new ApplicationException(_localizer.GetString(SharedResources.UnauthorizedAccess));
        }

        return userId;
    }
}
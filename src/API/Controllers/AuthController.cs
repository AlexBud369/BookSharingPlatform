using Application.Common;
using Application.DTOs;
using Application.DTOs.User;
using Application.Features.Auth.Commands;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly IValidator<RegisterDto> _registerValidator;
    private readonly IValidator<LoginDto> _loginValidator;
    private readonly IValidator<RefreshTokenDto> _refreshTokenValidator;

    public AuthController(
        IMediator mediator,
        IStringLocalizer<SharedResources> localizer,
        IValidator<RegisterDto> registerValidator,
        IValidator<LoginDto> loginValidator,
        IValidator<RefreshTokenDto> refreshTokenValidator)
    {
        _mediator = mediator;
        _localizer = localizer;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _refreshTokenValidator = refreshTokenValidator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto, CancellationToken cancellationToken)
    {
        var validationResult = await _registerValidator.ValidateAsync(registerDto, cancellationToken);
        if (!validationResult.IsValid) {
            return BadRequest(validationResult.Errors);
        }

        var command = new RegisterCommand
        {
            UserName = registerDto.Username,
            Email = registerDto.Email,
            Password = registerDto.Password
        };

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
    {
        var validationResult = await _loginValidator.ValidateAsync(loginDto, cancellationToken);
        if (!validationResult.IsValid) {
            return BadRequest(validationResult.Errors);
        }

        var command = new LoginCommand
        {
            Email = loginDto.Email,
            Password = loginDto.Password
        };

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken)
    {
        var validationResult = await _refreshTokenValidator.ValidateAsync(refreshTokenDto, cancellationToken);
        if (!validationResult.IsValid) {
            return BadRequest(validationResult.Errors);
        }

        var command = new RefreshTokenCommand
        {
            RefreshToken = refreshTokenDto.Token
        };

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId)) {
            return Unauthorized(_localizer.GetString(SharedResources.UnauthorizedAccess));
        }

        var command = new LogoutCommand
        {
            UserId = Guid.Parse(userId),
            RefreshToken = Request.Headers["Refresh-Token"].ToString()
        };

        await _mediator.Send(command, cancellationToken);

        return Ok();
    }
}
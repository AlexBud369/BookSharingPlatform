using Application.Common;
using Application.Features.Books.Commands;
using Application.Interfaces;
using Domain.Constants;
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
[Authorize]
public class FileController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISignedUrlService _signedUrlService;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public FileController(
        IMediator mediator,
        ISignedUrlService signedUrlService,
        IStringLocalizer<SharedResources> localizer)
    {
        _mediator = mediator;
        _signedUrlService = signedUrlService;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    [HttpDelete("{fileName}")]
    public async Task<IActionResult> DeleteFile(
        string fileName,
        [FromQuery] Guid bookId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteBookCover.Command
        {
            FileName = fileName,
            BookId = bookId,
            UserId = Guid.Parse(User.FindFirst(DomainConstants.Jwt.ClaimSub)?.Value ?? throw new ApplicationException(_localizer.GetString(SharedResources.UnauthorizedAccess)))
        };

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpGet("{fileName}/signed-url")]
    public async Task<IActionResult> GetSignedUrl(
        string fileName,
        [FromQuery] int expiresInSeconds = DomainConstants.Book.DefaultSignedUrlExpirationSeconds,
        CancellationToken cancellationToken = default)
    {
        Guard.AgainstEmptyString(fileName, nameof(fileName), _localizer.GetString(SharedResources.FileNameRequired));
        Guard.AgainstFalse(expiresInSeconds > 0, nameof(expiresInSeconds), _localizer.GetString(SharedResources.InvalidExpirationTime));

        var signedUrl = await _signedUrlService.GenerateSignedUrlAsync(fileName, expiresInSeconds, cancellationToken);
        
        return Ok(new { SignedUrl = signedUrl });
    }
}
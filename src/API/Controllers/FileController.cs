using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FileController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ISignedUrlService _signedUrlService;
    private readonly IBookAccessService _bookAccessService;
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public FileController(
        IFileStorageService fileStorageService,
        ISignedUrlService signedUrlService,
        IBookAccessService bookAccessService,
        IStringLocalizer<SharedResources> localizer,
        UserManager<ApplicationUser> userManager,
        AppDbContext context)
    {
        _fileStorageService = fileStorageService;
        _signedUrlService = signedUrlService;
        _bookAccessService = bookAccessService;
        _localizer = localizer;
        _userManager = userManager;
        _context = context;
        Guard.Initialize(_localizer);
    }

    [HttpDelete("{fileName}")]
    public async Task<IActionResult> DeleteFile(
        string fileName,
        [FromQuery] Guid bookId,
        CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyString(fileName, nameof(fileName), _localizer.GetString(SharedResources.FileNameRequired));
        Guard.AgainstEmptyGuid(bookId, nameof(bookId), _localizer.GetString(SharedResources.BookIdRequired));

        var user = await _userManager.GetUserAsync(User);
        Guard.AgainstNull(user, nameof(user), _localizer.GetString(SharedResources.UserNotFound), user.Id.ToString());

        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == bookId, cancellationToken);
        Guard.AgainstNull(book, nameof(bookId), _localizer.GetString(SharedResources.BookNotFound), bookId.ToString());

        await _bookAccessService.ValidateBookAccessAsync(book, user.Id, false, cancellationToken);

        await _fileStorageService.DeleteFileAsync(fileName, cancellationToken);
        book.CoverImageUrl = null;
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet("{fileName}/signed-url")]
    public async Task<IActionResult> GetSignedUrl(
        string fileName,
        [FromQuery] int expiresInSeconds = 3600,
        CancellationToken cancellationToken = default)
    {
        Guard.AgainstEmptyString(fileName, nameof(fileName), _localizer.GetString(SharedResources.FileNameRequired));
        Guard.AgainstFalse(expiresInSeconds > 0, nameof(expiresInSeconds), _localizer.GetString(SharedResources.InvalidExpirationTime));

        var signedUrl = await _signedUrlService.GenerateSignedUrlAsync(fileName, expiresInSeconds, cancellationToken);
        return Ok(new { SignedUrl = signedUrl });
    }
}
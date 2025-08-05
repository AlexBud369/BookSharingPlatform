using Application.Common;
using Application.DTOs.Book;
using Application.Features.Books.Commands;
using Application.Features.Books.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly IValidator<BookCreateDto> _createValidator;
    private readonly IValidator<BookUpdateDto> _updateValidator;
    private readonly IValidator<BookFilterDto> _filterValidator;
    private readonly IValidator<UploadBookCoverCommand> _uploadCoverValidator;

    public BooksController(
        IMediator mediator,
        IStringLocalizer<SharedResources> localizer,
        IValidator<BookCreateDto> createValidator,
        IValidator<BookUpdateDto> updateValidator,
        IValidator<BookFilterDto> filterValidator,
        IValidator<UploadBookCoverCommand> uploadCoverValidator)
    {
        _mediator = mediator;
        _localizer = localizer;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _filterValidator = filterValidator;
        _uploadCoverValidator = uploadCoverValidator;
        Guard.Initialize(_localizer);
    }

    [HttpGet]
    public async Task<IActionResult> GetBooks([FromQuery] BookFilterDto filterDto, CancellationToken cancellationToken)
    {
        var validationResult = await _filterValidator.ValidateAsync(filterDto, cancellationToken);
        if (!validationResult.IsValid) {
            return BadRequest(validationResult.Errors);
        }

        var query = new GetAllBooksQuery
        {
            Filter = filterDto
        };

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBook(Guid id, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(id, nameof(id), _localizer.GetString(SharedResources.BookIdRequired));
        var query = new GetBookByIdQuery
        {
            Id = id
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateBook([FromBody] BookCreateDto createDto, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(createDto, cancellationToken);
        if (!validationResult.IsValid) {
            return BadRequest(validationResult.Errors);
        }

        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new ApplicationException(_localizer.GetString(SharedResources.UnauthorizedAccess)));
        var command = new CreateBookCommand
        {
            Book = createDto,
            UserId = userId
        };

        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetBook), new { id = result.Id }, result);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(Guid id, [FromBody] BookUpdateDto updateDto, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(id, nameof(id), _localizer.GetString(SharedResources.BookIdRequired));
        var validationResult = await _updateValidator.ValidateAsync(updateDto, cancellationToken);
        if (!validationResult.IsValid) {
            return BadRequest(validationResult.Errors);
        }

        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new ApplicationException(_localizer.GetString(SharedResources.UnauthorizedAccess)));
        var command = new UpdateBookCommand
        {
            Id = id,
            Book = updateDto,
            UserId = userId
        };

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(Guid id, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(id, nameof(id), _localizer.GetString(SharedResources.BookIdRequired));
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new ApplicationException(_localizer.GetString(SharedResources.UnauthorizedAccess)));
        var command = new DeleteBookCommand
        {
            Id = id,
            UserId = userId
        };

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [Authorize]
    [HttpPost("{id}/cover")]
    public async Task<IActionResult> UploadBookCover(Guid id, [FromForm] IFormFile coverImage, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(id, nameof(id), _localizer.GetString(SharedResources.BookIdRequired));
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new ApplicationException(_localizer.GetString(SharedResources.UnauthorizedAccess)));
        var command = new UploadBookCoverCommand
        {
            Id = id,
            UserId = userId,
            CoverImage = coverImage
        };

        var validationResult = await _uploadCoverValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid) {
            return BadRequest(validationResult.Errors);
        }

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }
}
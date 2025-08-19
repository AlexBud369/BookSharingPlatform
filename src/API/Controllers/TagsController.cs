using Application.Common;
using Application.DTOs.Tag;
using Application.Features.Tags.Commands;
using Application.Features.Tags.Queries;
using Domain.Constants;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TagsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly IValidator<CreateTag> _createValidator;
    private readonly IValidator<GetAllTags> _getAllValidator;
    private readonly IValidator<GetTagById> _getByIdValidator;
    private readonly IValidator<DeleteTag> _deleteValidator;

    public TagsController(
        IMediator mediator,
        IStringLocalizer<SharedResources> localizer,
        IValidator<CreateTag> createValidator,
        IValidator<GetAllTags> getAllValidator,
        IValidator<GetTagById> getByIdValidator,
        IValidator<DeleteTag> deleteValidator)
    {
        _mediator = mediator;
        _localizer = localizer;
        _createValidator = createValidator;
        _getAllValidator = getAllValidator;
        _getByIdValidator = getByIdValidator;
        _deleteValidator = deleteValidator;
        Guard.Initialize(_localizer);
    }

    [HttpGet]
    public async Task<IActionResult> GetTags(
        [FromQuery] int pageNumber = DomainConstants.Tag.DefaultPageNumber,
        [FromQuery] int pageSize = DomainConstants.Tag.DefaultPageSize,
        [FromQuery] string? tagName = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllTags
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TagName = tagName
        };

        var validationResult = await _getAllValidator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid) {
            return BadRequest(validationResult.Errors);
        }

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTag(Guid id, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(id, nameof(id), _localizer.GetString(SharedResources.TagIdRequired));
        var query = new GetTagById { Id = id };
        var validationResult = await _getByIdValidator.ValidateAsync(query, cancellationToken);
        
        if (!validationResult.IsValid) {
            return BadRequest(validationResult.Errors);
        }

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateTag([FromBody] TagCreateDto createDto, CancellationToken cancellationToken)
    {
        var command = new CreateTag { TagName = createDto.TagName };
        var validationResult = await _createValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid) {
            return BadRequest(validationResult.Errors);
        }

        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetTag), new { id = result.Id }, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(Guid id, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(id, nameof(id), _localizer.GetString(SharedResources.TagIdRequired));
        var command = new DeleteTag { Id = id };
        var validationResult = await _deleteValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid) {
            return BadRequest(validationResult.Errors);
        }

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
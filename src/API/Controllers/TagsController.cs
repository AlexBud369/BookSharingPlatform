using Application.Common;
using Application.DTOs.Tag;
using Application.Features.Tags.Commands;
using Application.Features.Tags.Queries;
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
    private readonly IValidator<CreateTagCommand> _createValidator;
    private readonly IValidator<GetAllTagsQuery> _getAllValidator;
    private readonly IValidator<GetTagByIdQuery> _getByIdValidator;
    private readonly IValidator<DeleteTagCommand> _deleteValidator;

    public TagsController(
        IMediator mediator,
        IStringLocalizer<SharedResources> localizer,
        IValidator<CreateTagCommand> createValidator,
        IValidator<GetAllTagsQuery> getAllValidator,
        IValidator<GetTagByIdQuery> getByIdValidator,
        IValidator<DeleteTagCommand> deleteValidator)
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
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? tagName = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllTagsQuery
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
        var query = new GetTagByIdQuery { Id = id };
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
        var command = new CreateTagCommand { TagName = createDto.TagName };
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
        var command = new DeleteTagCommand { Id = id };
        var validationResult = await _deleteValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid) {
            return BadRequest(validationResult.Errors);
        }

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
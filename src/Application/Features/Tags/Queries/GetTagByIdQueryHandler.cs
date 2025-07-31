using Application.Common;
using Application.DTOs.Tag;
using Application.Interfaces;
using Application.Services;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tags.Queries;

public class GetTagByIdQueryHandler : IRequestHandler<GetTagByIdQuery, TagDto>
{
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly ITagQueryService _tagQueryService;

    public GetTagByIdQueryHandler(
        IStringLocalizer<SharedResources> localizer,
        ITagQueryService tagQueryService)
    {
        _localizer = localizer;
        _tagQueryService = tagQueryService;
        Guard.Initialize(_localizer);
    }

    public async Task<TagDto> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(request.Id, nameof(request.Id), _localizer.GetString(SharedResources.TagIdRequired));

        return await _tagQueryService.GetTagByIdAsync(request.Id, cancellationToken);
    }
}
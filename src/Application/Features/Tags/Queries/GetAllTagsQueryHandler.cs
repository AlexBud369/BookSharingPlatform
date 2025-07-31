using Application.Common;
using Application.DTOs.Tag;
using Application.Interfaces;
using Application.Services;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tags.Queries;

public class GetAllTagsQueryHandler : IRequestHandler<GetAllTagsQuery, IEnumerable<TagDto>>
{
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly ITagQueryService _tagQueryService;

    public GetAllTagsQueryHandler(
        IStringLocalizer<SharedResources> localizer,
        ITagQueryService tagQueryService)
    {
        _localizer = localizer;
        _tagQueryService = tagQueryService;
        Guard.Initialize(_localizer);
    }

    public async Task<IEnumerable<TagDto>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        Guard.AgainstInvalidPageNumber(request.PageNumber, nameof(request.PageNumber), _localizer.GetString(SharedResources.InvalidPageNumber));
        Guard.AgainstInvalidPageSize(request.PageSize, nameof(request.PageSize), _localizer.GetString(SharedResources.InvalidPageSize));

        return await _tagQueryService.GetTagsAsync(
            request.PageNumber,
            request.PageSize,
            request.TagName,
            cancellationToken);
    }
}
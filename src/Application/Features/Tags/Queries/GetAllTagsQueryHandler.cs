using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.DTOs.Tag;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Application.Features.Tags.Queries;

public class GetAllTagsQueryHandler : IRequestHandler<GetAllTagsQuery, IEnumerable<TagDto>>
{
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly ITagQueryService _tagQueryService;

    public GetAllTagsQueryHandler(
        IStringLocalizer<SharedResource> localizer,
        ITagQueryService tagQueryService)
    {
        _localizer = localizer;
        _tagQueryService = tagQueryService;
        Guard.Initialize(_localizer);
    }
    public async Task<IEnumerable<TagDto>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        return await _tagQueryService.GetTagsAsync(
           request.PageNumber,
           request.PageSize,
           request.TagName,
           cancellationToken);
    }
}
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.DTOs.Tag;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Application.Features.Tags.Queries;

public class GetTagByIdQueryHandler : IRequestHandler<GetTagByIdQuery, TagDto>
{
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly ITagQueryService _tagQueryService;

    public GetTagByIdQueryHandler(
         IStringLocalizer<SharedResource> localizer,
        ITagQueryService tagQueryService)
    {
        _localizer = localizer;
        _tagQueryService = tagQueryService;
        Guard.Initialize(_localizer);
    }
    public async Task<TagDto> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        return await _tagQueryService.GetTagByIdAsync(request.Id, cancellationToken);
    }
}
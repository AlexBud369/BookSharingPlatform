using Application.Common;
using Application.DTOs.Tag;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tags.Queries;

public class GetTagById : IRequest<TagDto>
{
    public Guid Id { get; set; }

    public class Handler
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ITagQueryService _tagQueryService;

        public Handler(
            IStringLocalizer<SharedResources> localizer,
            ITagQueryService tagQueryService)
        {
            _localizer = localizer;
            _tagQueryService = tagQueryService;
            Guard.Initialize(_localizer);
        }

        public async Task<TagDto> Handle(GetTagById request, CancellationToken cancellationToken)
        {
            Guard.AgainstEmptyGuid(request.Id, nameof(request.Id), _localizer.GetString(SharedResources.TagIdRequired));

            return await _tagQueryService.GetTagByIdAsync(request.Id, cancellationToken);
        }
    }
}
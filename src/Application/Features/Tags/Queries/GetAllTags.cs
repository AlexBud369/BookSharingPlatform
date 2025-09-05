using Application.Common;
using Application.DTOs;
using Application.DTOs.Tag;
using Application.Interfaces;
using Domain.Constants;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tags.Queries;

public class GetAllTags : IRequest<PagedResponseDto<TagDto>>
{
    public int PageNumber { get; set; } = DomainConstants.Tag.DefaultPageNumber;
    public int PageSize { get; set; } = DomainConstants.Tag.DefaultPageSize;
    public string? TagName { get; set; }

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

        public async Task<PagedResponseDto<TagDto>> Handle(GetAllTags request, CancellationToken cancellationToken)
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
}
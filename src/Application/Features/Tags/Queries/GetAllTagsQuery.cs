using Application.DTOs.Tag;
using Domain.Constants;
using MediatR;

namespace Application.Features.Tags.Queries;

public class GetAllTagsQuery : IRequest<IEnumerable<TagDto>>
{
    public int PageNumber { get; set; } = DomainConstants.Tag.DefaultPageNumber;
    public int PageSize { get; set; } = DomainConstants.Tag.DefaultPageSize;
    public string? TagName { get; set; }
}
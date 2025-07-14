using MediatR;
using Application.DTOs.Tag;

namespace Application.Features.Tags.Queries;

public class GetAllTagsQuery : IRequest<IEnumerable<TagDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? TagName { get; set; }
}
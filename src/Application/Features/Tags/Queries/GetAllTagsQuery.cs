using Application.DTOs.Tag;
using Domain.Constants;
using MediatR;

namespace Application.Features.Tags.Queries;

public class GetAllTagsQuery : IRequest<IEnumerable<TagDto>>
{
    public int PageNumber { get; set; } = DomainConstants.Book.DefaultPageNumber;
    public int PageSize { get; set; } = DomainConstants.Book.DefaultPageSize;
    public string? TagName { get; set; }
}
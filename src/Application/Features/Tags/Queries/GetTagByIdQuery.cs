using MediatR;
using Application.DTOs.Tag;

namespace Application.Features.Tags.Queries;

public class GetTagByIdQuery : IRequest<TagDto>
{
    public Guid Id { get; set; }
}
using MediatR;
using Application.DTOs.Tag;

namespace Application.Features.Tags.Commands;

public class CreateTagCommand : IRequest<TagDto>
{
    public string TagName { get; set; } = string.Empty;
}
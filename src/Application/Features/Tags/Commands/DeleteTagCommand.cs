using MediatR;
using System;

namespace Application.Features.Tags.Commands;

public class DeleteTagCommand : IRequest
{
    public Guid Id { get; set; }
}
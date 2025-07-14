using Application.Common.Exceptions;
using Application.DTOs.Tag;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;

namespace Application.Features.Tags.Commands;

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, TagDto>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CreateTagCommandHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<TagDto> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var existingTag = await _context.Tags
            .FirstOrDefaultAsync(t => t.tagName == request.TagName, cancellationToken);
        if (existingTag != null)
        {
            throw new UnauthorizedAccessException($"Tag with name {request.TagName} already exists");
        }

        var tag = new Tag { tagName = request.TagName };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<TagDto>(tag);
    }
}
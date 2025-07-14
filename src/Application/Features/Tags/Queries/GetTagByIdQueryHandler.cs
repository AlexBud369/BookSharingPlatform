using Application.Common.Exceptions;
using Application.DTOs.Tag;
using AutoMapper;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;

namespace Application.Features.Tags.Queries;

public class GetTagByIdQueryHandler : IRequestHandler<GetTagByIdQuery, TagDto>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public GetTagByIdQueryHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<TagDto> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.tagId == request.Id, cancellationToken);
        if (tag == null)
        {
            throw new TagNotFoundException(request.Id);
        }
        return _mapper.Map<TagDto>(tag);
    }
}
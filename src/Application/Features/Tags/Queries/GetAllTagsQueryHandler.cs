using Application.DTOs.Tag;
using AutoMapper;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;

namespace Application.Features.Tags.Queries;

public class GetAllTagsQueryHandler : IRequestHandler<GetAllTagsQuery, IEnumerable<TagDto>>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public GetAllTagsQueryHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<TagDto>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Tags.AsQueryable();

        if (!string.IsNullOrEmpty(request.TagName))
            query = query.Where(t => t.tagName.Contains(request.TagName));

        var tags = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<TagDto>>(tags);
    }
}
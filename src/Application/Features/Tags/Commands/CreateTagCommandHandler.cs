using Application.Common;
using Application.DTOs.Tag;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tags.Commands;

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, TagDto>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public CreateTagCommandHandler(
        AppDbContext context,
        IMapper mapper,
        IStringLocalizer<SharedResources> localizer)
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    public async Task<TagDto> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyString(request.TagName, nameof(request.TagName), _localizer.GetString(SharedResources.TagNameRequired));

        var existingTag = await _context.Tags
            .FirstOrDefaultAsync(t => t.TagName == request.TagName, cancellationToken);
        Guard.AgainstNull(existingTag == null, nameof(request.TagName), _localizer.GetString(SharedResources.TagAlreadyExists), request.TagName);

        var tag = new Tag { TagName = request.TagName };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TagDto>(tag);
    }
}
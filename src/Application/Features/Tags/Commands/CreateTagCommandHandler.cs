using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.DTOs.Tag;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Application.Features.Tags.Commands;

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, TagDto>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public CreateTagCommandHandler(
        AppDbContext context,
        IMapper mapper,
        IStringLocalizer<SharedResource> localizer)
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }
    public async Task<TagDto> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyString(request.TagName, nameof(request.TagName), "TagNameRequired");

        var existingTag = await _context.Tags
            .FirstOrDefaultAsync(t => t.tagName == request.TagName, cancellationToken);
        Guard.Against(existingTag != null, nameof(request.TagName), "TagAlreadyExists", request.TagName);

        var tag = new Tag { tagName = request.TagName };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<TagDto>(tag);
    }
}
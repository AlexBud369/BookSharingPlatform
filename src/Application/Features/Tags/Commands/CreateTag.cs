using Application.Common;
using Application.DTOs.Tag;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Persistence.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tags.Commands;

public class CreateTag : IRequest<TagDto> 
{
    public string TagName { get; set; } = string.Empty;

    public class Handler
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public Handler(
            AppDbContext context,
            IMapper mapper,
            IStringLocalizer<SharedResources> localizer)
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
            Guard.Initialize(_localizer);
        }

        public async Task<TagDto> Handle(CreateTag request, CancellationToken cancellationToken)
        {
            Guard.AgainstEmptyString(request.TagName, nameof(request.TagName), _localizer.GetString(SharedResources.TagNameRequired));
            Guard.AgainstMaxLength(request.TagName, DomainConstants.Tag.NameMaxLength, nameof(request.TagName), _localizer.GetString(SharedResources.TagNameTooLong));

            var existingTag = await _context.Tags
                .FirstOrDefaultAsync(t => t.TagName == request.TagName, cancellationToken);
            Guard.AgainstNull(existingTag == null, nameof(request.TagName), _localizer.GetString(SharedResources.TagAlreadyExists), request.TagName);

            var tag = new Tag { TagName = request.TagName };
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<TagDto>(tag);
        }
    }
}
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Persistence.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tags.Commands;

public class DeleteTag : IRequest<Unit>
{
    public Guid Id { get; set; }

    public class Handler
    {
        private readonly AppDbContext _context;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public Handler(
            AppDbContext context,
            IStringLocalizer<SharedResources> localizer)
        {
            _context = context;
            _localizer = localizer;
            Guard.Initialize(_localizer);
        }

        public async Task<Unit> Handle(DeleteTag request, CancellationToken cancellationToken)
        {
            Guard.AgainstEmptyGuid(request.Id, nameof(request.Id), _localizer.GetString(SharedResources.TagIdRequired));

            var tag = await _context.Tags
                .Include(t => t.Books)
                .FirstOrDefaultAsync(t => t.TagId == request.Id, cancellationToken);
            Guard.AgainstNull(tag, nameof(request.Id), _localizer.GetString(SharedResources.TagNotFound), request.Id.ToString());

            Guard.AgainstFalse(tag.Books.Count == 0, nameof(request.Id), _localizer.GetString(SharedResources.TagCannotBeDeleted), request.Id.ToString());

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
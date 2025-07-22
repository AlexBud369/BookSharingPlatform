using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class TagService : ITagService
{
    private readonly AppDbContext _context;

    public TagService(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddTagsToBookAsync(Book book, IEnumerable<string> tagNames, CancellationToken cancellationToken)
    {
        if (tagNames == null || !tagNames.Any()) {
            return;
        }

        var tagNamesList = tagNames.ToList();
        var existingTags = await _context.Tags
            .Where(t => tagNamesList.Contains(t.tagName))
            .ToListAsync(cancellationToken);
        book.TagsList.AddRange(existingTags);

        var newTagNames = tagNamesList.Except(existingTags.Select(t => t.tagName)).ToList();
        book.TagsList.AddRange(newTagNames.Select(name => new Tag { tagName = name }));
    }
}

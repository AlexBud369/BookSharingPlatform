using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Application.Services;

public class TagService : ITagService
{
    private readonly AppDbContext _context;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public TagService(
        AppDbContext context,
        IStringLocalizer<SharedResources> localizer)
    {
        _context = context;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    public async Task AddTagsToBookAsync(Book book, IEnumerable<string> tagNames, CancellationToken cancellationToken)
    {
        ValidateInputs(book, tagNames);

        var tagNamesList = tagNames.ToList();
        if (!tagNamesList.Any()) {
            return;
        }

        await ClearExistingBookTagsAsync(book.Id, cancellationToken);
        var tags = await GetOrCreateTagsAsync(tagNamesList, cancellationToken);
        await AddBookTagsAsync(book.Id, tags, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    private void ValidateInputs(Book book, IEnumerable<string> tagNames)
    {
        Guard.AgainstNull(book, nameof(book), _localizer.GetString(SharedResources.BookNotFound));
        Guard.AgainstEmptyGuid(book.Id, nameof(book.Id), _localizer.GetString(SharedResources.BookIdRequired));
        Guard.AgainstNull(tagNames, nameof(tagNames), _localizer.GetString(SharedResources.TagsRequired));

        foreach (var tagName in tagNames) {
            Guard.AgainstEmptyString(tagName, nameof(tagName), _localizer.GetString(SharedResources.TagNameRequired));
        }
    }

    private async Task ClearExistingBookTagsAsync(Guid bookId, CancellationToken cancellationToken)
    {
        var bookTagEntries = await _context.Set<Dictionary<string, object>>("BookTag")
            .Where(bt => (Guid)bt["BookId"] == bookId)
            .ToListAsync(cancellationToken);
        _context.RemoveRange(bookTagEntries);
    }

    private async Task<List<Tag>> GetOrCreateTagsAsync(List<string> tagNames, CancellationToken cancellationToken)
    {
        var existingTags = await _context.Tags
            .Where(t => tagNames.Contains(t.TagName))
            .ToListAsync(cancellationToken);

        var newTagNames = tagNames.Except(existingTags.Select(t => t.TagName)).ToList();
        var newTags = newTagNames.Select(name => new Tag { TagName = name }).ToList();
        _context.Tags.AddRange(newTags);

        return existingTags.Concat(newTags).ToList();
    }

    private async Task AddBookTagsAsync(Guid bookId, List<Tag> tags, CancellationToken cancellationToken)
    {
        var bookTagEntries = tags.Select(tag => new Dictionary<string, object>
        {
            { "BookId", bookId },
            { "TagId", tag.TagId }
        }).ToList();

        foreach (var entry in bookTagEntries) {
            await _context.Set<Dictionary<string, object>>("BookTag").AddAsync(entry, cancellationToken);
        }
    }

    public async Task<IEnumerable<string>> GetTagNamesByIdsAsync(IEnumerable<Guid> tagIds, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(tagIds, nameof(tagIds), _localizer.GetString(SharedResources.TagsRequired));

        var tagIdsList = tagIds.ToList();
        if (!tagIdsList.Any()) {
            return Enumerable.Empty<string>();
        }

        foreach (var tagId in tagIdsList) {
            Guard.AgainstEmptyGuid(tagId, nameof(tagId), _localizer.GetString(SharedResources.TagIdRequired));
        }

        var tags = await _context.Tags
            .Where(t => tagIdsList.Contains(t.TagId))
            .Select(t => t.TagName)
            .ToListAsync(cancellationToken);

        return tags;
    }
}
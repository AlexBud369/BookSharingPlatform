using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
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
        Guard.AgainstNull(context, nameof(context), localizer.GetString(SharedResources.DbContextRequired));
        Guard.AgainstNull(localizer, nameof(localizer), localizer.GetString(SharedResources.LocalizerRequired));

        _context = context;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    public async Task AddTagsToBookAsync(Book book, IEnumerable<string> tagNames, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(book, nameof(book), _localizer.GetString(SharedResources.BookNotFound));
        Guard.AgainstEmptyGuid(book.Id, nameof(book.Id), _localizer.GetString(SharedResources.BookIdRequired));
        Guard.AgainstNull(tagNames, nameof(tagNames), _localizer.GetString(SharedResources.TagsRequired));

        var tagNamesList = tagNames.ToList();
        if (!tagNamesList.Any())
        {
            return;
        }

        foreach (var tagName in tagNamesList)
        {
            Guard.AgainstEmptyString(tagName, nameof(tagName), _localizer.GetString(SharedResources.TagNameRequired));
        }

        var existingTags = await _context.Tags
            .Where(t => tagNamesList.Contains(t.TagName))
            .ToListAsync(cancellationToken);
        book.Tags.AddRange(existingTags);

        var newTagNames = tagNamesList.Except(existingTags.Select(t => t.TagName)).ToList();
        book.Tags.AddRange(newTagNames.Select(name => new Tag { TagName = name }));
    }
}
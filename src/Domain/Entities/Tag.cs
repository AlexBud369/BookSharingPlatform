using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Entities;

/// <summary>
/// The entity of a tag is used to categorize books (genre, language)
/// </summary>
public class Tag
{
    public Guid tagId { get; init; } = Guid.NewGuid();
    public string tagName { get; private set; }
    public IReadOnlyList<Book> Books { get => BooksList.AsReadOnly(); }
    private readonly List<Book> BooksList = new();

    private Tag() { }

    public Tag(Guid tagId, string tagName)
    {
        this.tagName = tagName;
    }

}


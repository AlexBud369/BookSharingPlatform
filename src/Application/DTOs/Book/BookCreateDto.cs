using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Book;

public class BookCreateDto
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string? Description { get; set; }
    public IEnumerable<string> Tags { get; set; }
}

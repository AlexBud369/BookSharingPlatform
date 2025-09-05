using Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Book;

public class BookFilterDto
{
    public string? SearchQuery { get; set; }
    public IEnumerable<Guid>? TagIds { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int PageNumber { get; set; } = DomainConstants.Book.DefaultPageNumber;
    public int PageSize { get; set; } = DomainConstants.Book.DefaultPageSize;
}
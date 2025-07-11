using MediatR;
using Application.DTOs.Book;

namespace Application.Features.Books.Queries;

public class GetAllBooksQuery : IRequest<IEnumerable<BookDto>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SearchTerm { get; set; }
    public string? Tag { get; set; }
}

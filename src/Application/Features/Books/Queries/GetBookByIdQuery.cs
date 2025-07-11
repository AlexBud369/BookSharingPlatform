using MediatR;
using Application.DTOs.Book;

namespace Application.Features.Books.Queries;

public class GetBookByIdQuery : IRequest<BookDto>
{
    public Guid Id { get; set; }
}
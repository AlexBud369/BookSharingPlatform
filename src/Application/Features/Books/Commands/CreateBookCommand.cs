using MediatR;
using Application.DTOs.Book;

namespace Application.Features.Books.Commands;

public class CreateBookCommand : IRequest<BookDto>
{
    public BookCreateDto Book { get; set; } = new BookCreateDto();
    public Guid UserId { get; set; }
}

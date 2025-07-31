using MediatR;
using Application.DTOs.Book;

namespace Application.Features.Books.Commands;

public class UpdateBookCommand : IRequest<BookDto>
{
    public Guid Id { get; set; }
    public BookUpdateDto Book { get; set; } = new BookUpdateDto();
    public Guid UserId { get; set; }
}

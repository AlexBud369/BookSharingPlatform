using MediatR;
using Application.DTOs.Book;

namespace Application.Features.Books.Commands;

public class UploadBookCoverCommand : IRequest<BookDto>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;
}
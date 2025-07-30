using MediatR;
using Application.DTOs.Book;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Books.Commands;

public class UploadBookCoverCommand : IRequest<BookDto>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public IFormFile CoverImage { get; set; }
}
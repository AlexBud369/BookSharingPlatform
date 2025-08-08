using MediatR;

namespace Application.Features.Books.Commands;

public class DeleteBookCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}

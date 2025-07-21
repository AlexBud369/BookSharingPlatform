using Application.Common.Exceptions;
using Application.DTOs.Book;
using Application.Services;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.DTOs.Book;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Application.Features.Books.Queries;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto>
{
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly IBookQueryService _bookQueryService;

    public GetBookByIdQueryHandler(
        IStringLocalizer<SharedResource> localizer,
        IBookQueryService bookQueryService)
    {
        _localizer = localizer;
        _bookQueryService = bookQueryService;
        Guard.Initialize(_localizer);
    }

    public async Task<BookDto> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        return await _bookQueryService.GetBookByIdAsync(request.Id, cancellationToken);
    }
}

using Application.Common;
using Application.DTOs.Book;
using Application.Interfaces;
using Application.Services;
using MediatR;
using Microsoft.Extensions.Localization;
using System;

namespace Application.Features.Books.Queries;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto>
{
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly IBookQueryService _bookQueryService;

    public GetBookByIdQueryHandler(
        IStringLocalizer<SharedResources> localizer,
        IBookQueryService bookQueryService)
    {
        _localizer = localizer;
        _bookQueryService = bookQueryService;
        Guard.Initialize(_localizer);
    }

    public async Task<BookDto> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(
            request.Id,
            nameof(request.Id),
            _localizer.GetString(SharedResources.BookIdRequired));

        return await _bookQueryService.GetBookByIdAsync(request.Id, cancellationToken);
    }
}

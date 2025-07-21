using Application.Common;
using Application.DTOs.Book;
using Application.Interfaces;
using Application.Services;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Books.Queries;

public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, IEnumerable<BookDto>>
{
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly IBookQueryService _bookQueryService;

    public GetAllBooksQueryHandler(
        IStringLocalizer<SharedResource> localizer,
        IBookQueryService bookQueryService)
    {
        _localizer = localizer;
        _bookQueryService = bookQueryService;
        Guard.Initialize(_localizer);
    }

    public async Task<IEnumerable<BookDto>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        return await _bookQueryService.GetBooksAsync(
             request.PageNumber,
             request.PageSize,
             request.SearchTerm,
             request.Tag,
             cancellationToken);
    }
}

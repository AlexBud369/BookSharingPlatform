using Application.Common;
using Application.DTOs.Book;
using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Books.Queries;

public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, PagedResponseDto<BookDto>>
{
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly IBookQueryService _bookQueryService;

    public GetAllBooksQueryHandler(
        IStringLocalizer<SharedResource> localizer,
        IBookQueryService bookQueryService)
    {
        Guard.AgainstNull(localizer, nameof(localizer), "LocalizerRequired");
        Guard.AgainstNull(bookQueryService, nameof(bookQueryService), "BookQueryServiceRequired");

        _localizer = localizer;
        _bookQueryService = bookQueryService;
        Guard.Initialize(_localizer);
    }

    public async Task<PagedResponseDto<BookDto>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(request.Filter, nameof(request.Filter), "FilterRequired");

        return await _bookQueryService.GetBooksAsync(
            request.Filter.PageNumber,
            request.Filter.PageSize,
            request.Filter.SearchQuery,
            request.Filter.TagIds,
            request.Filter.CreatedByUserId,
            request.Filter.SortBy,
            request.Filter.SortDescending,
            cancellationToken);
    }
}

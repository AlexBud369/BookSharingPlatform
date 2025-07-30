using System;
using System.Collections.Generic;
using Application.DTOs;
using Application.DTOs.Book;
using Domain.Constants;
using MediatR;

namespace Application.Features.Books.Queries;

public class GetAllBooksQuery : IRequest<PagedResponseDto<BookDto>>
{
    public BookFilterDto Filter { get; set; } = new BookFilterDto();

}

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.DTOs.Book;
using Domain.Entities;
using Infrastructure.Data;

namespace Application.Features.Books.Queries;

public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, IEnumerable<BookDto>>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public GetAllBooksQueryHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BookDto>> Handle(GetAllBooksQueryHandler request, CancellationToken cancellationToken)
    {
        var books = await _bookRepository.GetAllAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            request.Tag);

        return _mapper.Map<IEnumerable<BookDto>>(books);
    }
}

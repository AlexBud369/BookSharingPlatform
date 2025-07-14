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

    public async Task<IEnumerable<BookDto>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Books.AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(b => b.Title.Contains(request.SearchTerm) || b.Author.Contains(request.SearchTerm));
        }

        if (!string.IsNullOrEmpty(request.Tag))
        {
            query = query.Where(b => b.Tags.Any(t => t.tagName == request.Tag));
        }

        var books = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<BookDto>>(books);
    }
}

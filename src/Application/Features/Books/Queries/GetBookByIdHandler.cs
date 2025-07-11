using Application.DTOs.Book;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using System;

namespace Application.Features.Books.Queries;

public class GetBookByIdHandler : IRequestHandler<GetBookByIdQuery, BookDto>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public GetBookByIdQueryHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BookDto> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .Include(b => b.Tags)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        if (book == null)
        { 
            throw new KeyNotFoundException($"Book with ID {request.Id} not found.");
        }
        return _mapper.Map<BookDto>(book);
    }
}

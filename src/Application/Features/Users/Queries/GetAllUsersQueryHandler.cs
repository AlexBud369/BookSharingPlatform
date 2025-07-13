using Application.Common.Exceptions;
using Application.DTOs.User;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;

namespace Application.Features.Users.Queries;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(request.Email))
        {
            query = query.Where(u => u.Email.Contains(request.Email));
        }
        if (!string.IsNullOrEmpty(request.UserName))
        {
            query = query.Where(u => u.UserName.Contains(request.UserName));
        }
        if (request.IsBlocked.HasValue)
        {
            query = query.Where(u => u.IsBlocked == request.IsBlocked.Value);
        }

        var users = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }
}
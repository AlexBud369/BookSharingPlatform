using Application.Common;
using Application.DTOs.Book;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Books.Commands;

public class UploadBookCoverCommandHandler : IRequestHandler<UploadBookCoverCommand, BookDto>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly IBookAccessService _bookAccessService;
    private readonly IImageService _imageService;

    public UploadBookCoverCommandHandler(
        AppDbContext context, 
        IMapper mapper,
        IStringLocalizer<SharedResource> localizer,
        IBookAccessService bookAccessService,
        IImageService imageService)
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
        _bookAccessService = bookAccessService;
        _imageService = imageService;
        Guard.Initialize(_localizer);
    }

    public async Task<BookDto> Handle(UploadBookCoverCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        Guard.AgainstNull(user, nameof(request.UserId), "UserNotFound", request.UserId.ToString());

        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        Guard.AgainstNull(book, nameof(request.Id), "BookNotFound", request.Id.ToString());

        await _bookAccessService.ValidateBookAccessAsync(book, request.UserId, false, cancellationToken);
        await _imageService.UpdateBookCoverAsync(book, request.CoverImageUrl, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<BookDto>(book);
    }
}
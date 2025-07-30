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
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly IBookAccessService _bookAccessService;
    private readonly IImageService _imageService;

    public UploadBookCoverCommandHandler(
        AppDbContext context,
        IMapper mapper,
        IStringLocalizer<SharedResources> localizer,
        IBookAccessService bookAccessService,
        IImageService imageService)
    {
        Guard.AgainstNull(context, nameof(context), localizer.GetString(SharedResources.DbContextRequired));
        Guard.AgainstNull(mapper, nameof(mapper), localizer.GetString(SharedResources.MapperRequired));
        Guard.AgainstNull(localizer, nameof(localizer), localizer.GetString(SharedResources.LocalizerRequired));
        Guard.AgainstNull(bookAccessService, nameof(bookAccessService), localizer.GetString(SharedResources.BookAccessServiceRequired));
        Guard.AgainstNull(imageService, nameof(imageService), localizer.GetString(SharedResources.ImageServiceRequired));

        _context = context;
        _mapper = mapper;
        _localizer = localizer;
        _bookAccessService = bookAccessService;
        _imageService = imageService;
        Guard.Initialize(_localizer);
    }

    public async Task<BookDto> Handle(UploadBookCoverCommand request, CancellationToken cancellationToken)
    {
        Guard.AgainstEmptyGuid(request.Id, nameof(request.Id), _localizer.GetString(SharedResources.BookIdRequired));
        Guard.AgainstEmptyGuid(request.UserId, nameof(request.UserId), _localizer.GetString(SharedResources.UserIdRequired));
        Guard.AgainstNull(request.CoverImage, nameof(request.CoverImage), _localizer.GetString(SharedResources.FileStreamRequired));

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        Guard.AgainstNull(user, nameof(request.UserId), _localizer.GetString(SharedResources.UserNotFound), request.UserId.ToString());

        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        Guard.AgainstNull(book, nameof(request.Id), _localizer.GetString(SharedResources.BookNotFound), request.Id.ToString());

        await _bookAccessService.ValidateBookAccessAsync(book, request.UserId, false, cancellationToken);

        using var stream = request.CoverImage.OpenReadStream();
        await _imageService.UpdateBookCoverAsync(book, stream, request.CoverImage.FileName, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<BookDto>(book);
    }
}
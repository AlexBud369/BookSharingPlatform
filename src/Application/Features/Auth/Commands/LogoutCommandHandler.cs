using Application.Common;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;

namespace Application.Features.Auth.Commands;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly AppDbContext _context;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public LogoutCommandHandler(AppDbContext context, IStringLocalizer<SharedResources> localizer)
    {
        _context = context;
        _localizer = localizer;
        Guard.Initialize(_localizer);
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(
                rt => rt.UserId == request.UserId
                && rt.Token == request.RefreshToken
                && !rt.IsRevoked, cancellationToken);

        Guard.AgainstNull(
            refreshToken,
            nameof(request.RefreshToken),
            _localizer.GetString(SharedResources.RefreshTokenNotFound));

        refreshToken.IsRevoked = true;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
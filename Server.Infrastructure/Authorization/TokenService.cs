﻿using Server.Application.Common.Interfaces.Authentication;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Domain.Entity.Token;
using Server.Infrastructure.Persistence;
using System.Security.Cryptography;

namespace Server.Infrastructure.Authorization;

public class TokenService : ITokenService
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public TokenService(IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork)
    {
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public string GenerateRefreshToken()
    {
        var bytes = new byte[32];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }

    public async Task StoreRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var tokenEntity = new RefreshToken()
        {
            UserId = userId,
            Token = refreshToken,
            RefreshTokenExpiryTime = _dateTimeProvider.UtcNow().AddMinutes(60),
        };

        _unitOfWork.TokenRepository.Add(tokenEntity);

        await _unitOfWork.CompleteAsync();
    }
}

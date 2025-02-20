﻿namespace Server.Application.Common.Interfaces.Services;

public interface IUserService
{
    Guid GetUserId();

    bool? IsAuthenticated();
}

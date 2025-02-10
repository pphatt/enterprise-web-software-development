﻿using Server.Application.Common.Interfaces.Persistence.Repositories;

namespace Server.Application.Common.Interfaces.Persistence;

public interface IUnitOfWork
{
    Task<int> CompleteAsync();

    ITokenRepository TokenRepository { get; }
    IFacultyRepository FacultyRepository { get; }
}

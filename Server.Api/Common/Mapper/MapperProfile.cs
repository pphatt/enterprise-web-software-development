﻿using AutoMapper;
using Server.Application.Features.Authentication.Commands.Login;
using Server.Application.Features.Authentication.Commands.RefreshToken;
using Server.Contracts.Authentication.Login;
using Server.Contracts.Authentication.RefreshToken;

namespace Server.Api.Common.Mapper;

public class MapperProfile : Profile
{
    public MapperProfile() 
    {
        // Authentication.
        CreateMap<LoginRequest, LoginCommand>();
        CreateMap<RefreshTokenRequest, RefreshTokenCommand>();
    }
}

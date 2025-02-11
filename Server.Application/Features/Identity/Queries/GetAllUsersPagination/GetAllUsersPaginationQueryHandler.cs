﻿using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Dtos.Identity.Users;
using Server.Application.Common.Interfaces.Persistence;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Identity.Queries.GetAllUsersPagination;

public class GetAllUsersPaginationQueryHandler : IRequestHandler<GetAllUsersPaginationQuery, ErrorOr<List<UserDto>>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllUsersPaginationQueryHandler(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ErrorOr<List<UserDto>>> Handle(GetAllUsersPaginationQuery request, CancellationToken cancellationToken)
    {
        var allUserQuery = _userManager.Users;

        var count = await allUserQuery.CountAsync();

        var skipPages = (request.PageIndex - 1) * request.PageSize;

        allUserQuery =
            allUserQuery
                .Skip(skipPages)
                .Take(request.PageSize)
                .OrderBy(x => x.DateCreated);

        var result = await _mapper.ProjectTo<UserDto>(allUserQuery).ToListAsync(cancellationToken);

        foreach (var userDto in result)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());

            if (user is null)
            {
                continue;
            }

            var roles = await _userManager.GetRolesAsync(user);
            userDto.Roles = roles;

            if (user.FacultyId is not null)
            {
                var faculty = await _unitOfWork.FacultyRepository.GetByIdAsync(user.FacultyId.Value);

                userDto.Faculty = faculty.Name ?? null;
            }
        }

        return result;
    }
}

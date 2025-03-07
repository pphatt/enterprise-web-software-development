﻿using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.PublicContributionApp.Queries.GetAllReadLaterPagination;

public class GetAllReadLaterPaginationQueryHandler : IRequestHandler<GetAllReadLaterPaginationQuery, ErrorOr<ResponseWrapper<PaginationResult<PublicContributionInListDto>>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public GetAllReadLaterPaginationQueryHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ErrorOr<ResponseWrapper<PaginationResult<PublicContributionInListDto>>>> Handle(GetAllReadLaterPaginationQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        var result = await _unitOfWork.ContributionPublicReadLaterRepository.GetAllReadLaterPublicContributionPagination(
            keyword: request.Keyword,
            pageIndex: request.PageIndex,
            pageSize: request.PageSize,
            userId: request.UserId,
            facultyName: request.FacultyName,
            academicYearName: request.AcademicYearName,
            orderBy: request.OrderBy
        );

        foreach (var item in result.Results)
        {
            item.AlreadyLike = await _unitOfWork.LikeRepository.AlreadyLike(item.Id, user.Id);
        }

        return new ResponseWrapper<PaginationResult<PublicContributionInListDto>>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}

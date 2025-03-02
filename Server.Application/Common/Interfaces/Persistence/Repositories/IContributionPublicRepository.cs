﻿using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface IContributionPublicRepository : IRepository<ContributionPublic, Guid>
{
    Task<PaginationResult<PublicContributionInListDto>> GetAllPublicContributionsPagination(string? keyword, int pageIndex = 1, int pageSize = 10, string? academicYearName = null, string? facultyName = null, bool? allowedGuest = null);

    Task<PublicContributionDetailsDto> GetPublicContributionBySlug(string slug);
}

﻿using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface IContributionRepository : IRepository<Contribution, Guid>
{
    Task<bool> IsSlugAlreadyExisted(string slug, Guid? contributionId = null);

    Task<PaginationResult<ContributionInListDto>> GetAllContributionsPagination(string? keyword, int pageIndex = 1, int pageSize = 10, string? academicYear = null, string? faculty = null, string? status = null);

    Task SendToApproved(Guid contributionId, Guid studentId);

    Task ApproveContribution(Contribution contribution, Guid coordinatorId);

    Task RejectContribution(Contribution contribution, Guid coordinatorId, string reason);
}

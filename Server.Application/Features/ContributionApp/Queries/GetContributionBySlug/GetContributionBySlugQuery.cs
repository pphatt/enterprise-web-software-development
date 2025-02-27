﻿using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;
using Server.Contracts.Common;

namespace Server.Application.Features.ContributionApp.Queries.GetContributionBySlug;

public class GetContributionBySlugQuery : IRequest<ErrorOr<ResponseWrapper<ContributionDto>>>
{
    public string Slug { get; set; } = default!;

    public Guid UserId { get; set; }

    public string FacultyName { get; set; } = default!;
}

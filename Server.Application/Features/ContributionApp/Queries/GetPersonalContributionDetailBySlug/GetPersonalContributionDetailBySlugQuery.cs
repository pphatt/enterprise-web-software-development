﻿using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Wrapper;

namespace Server.Application.Features.ContributionApp.Queries.GetPersonalContributionDetailBySlug;

public class GetPersonalContributionDetailBySlugQuery : IRequest<ErrorOr<ResponseWrapper<ContributionWithCommentDto>>>
{
    public string Slug { get; set; } = default!;

    public Guid UserId { get; set; } = default!;
}

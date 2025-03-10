﻿using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.TagApp.Commands.DeleteTag;

public class DeleteTagCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid Id { get; set; }
}

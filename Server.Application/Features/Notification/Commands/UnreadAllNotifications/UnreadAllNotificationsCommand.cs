﻿using ErrorOr;

using MediatR;

using Server.Application.Wrapper;

namespace Server.Application.Features.Notification.Commands.UnreadAllNotifications;

public class UnreadAllNotificationsCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid UserId { get; set; }
}

using MediatR;
using System;

namespace WebApi.Apis.V1
{
    public record UserNotificationSeenUpdateCommand(
        string UserId,
        Guid NotificationId

    ) : IRequest;
}
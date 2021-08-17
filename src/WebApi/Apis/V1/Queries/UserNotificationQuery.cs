using MediatR;
using System;

namespace WebApi.Apis.V1
{
    public record UserNotificationQuery(
        string UserId,
        Guid NotificationId

    ) : IRequest<NotificationDto>;
}
using MediatR;

namespace WebApi.Apis.V1
{
    public record UserNotificationSettingsQuery(
        string UserId

    ) : IRequest<UserNotificationSettingsDto>;
}
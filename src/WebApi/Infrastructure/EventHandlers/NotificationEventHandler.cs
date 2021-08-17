using Hangfire;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Domain.Notifications;
using WebApi.Infrastructure.BackgroundJobs;

namespace WebApi.Infrastructure.EventHandlers
{
    public class NotificationEventHandler : INotificationHandler<NotificationCreatedEvent>
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IBackgroundJobHandler _backgroundJobHandler;

        public NotificationEventHandler(IBackgroundJobClient backgroundJobClient, IBackgroundJobHandler backgroundJobHandler)
        {
            _backgroundJobClient = backgroundJobClient;
            _backgroundJobHandler = backgroundJobHandler;
        }

        public Task Handle(NotificationCreatedEvent notification, CancellationToken cancellationToken)
        {
            _backgroundJobClient.Enqueue(() =>
                _backgroundJobHandler.HandleNotificationCreatedJobAsync(notification.NotificationId));

            return Task.CompletedTask;
        }
    }
}
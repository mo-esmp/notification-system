using System;
using System.Threading.Tasks;

namespace WebApi.Infrastructure.BackgroundJobs
{
    public interface IBackgroundJobHandler
    {
        Task HandleNotificationCreatedJobAsync(Guid notificationId);

        Task HandleBirthDayNotificationJobAsync();
    }
}
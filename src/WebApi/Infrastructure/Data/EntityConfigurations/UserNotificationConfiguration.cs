using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Domain.Notifications;
using WebApi.Domain.Users;

namespace WebApi.Infrastructure.Data.EntityConfigurations
{
    internal class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
    {
        public void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            builder.HasKey(un => new { un.UserId, un.NotificationId });

            builder.HasOne<User>().WithMany().HasForeignKey(un => un.UserId);
            builder.HasOne<Notification>().WithMany().HasForeignKey(un => un.NotificationId);
        }
    }
}
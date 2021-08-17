using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApi.Domain.Departments;
using WebApi.Domain.Notifications;

namespace WebApi.Infrastructure.Data.EntityConfigurations
{
    internal class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.Property(n => n.Title).IsUnicode().HasMaxLength(64).IsRequired();
            builder.Property(n => n.Message).IsUnicode().HasMaxLength(512).IsRequired();
            builder.Property(n => n.CreatorUserId).IsUnicode(false).HasMaxLength(256).IsRequired(false);

            var enumConverter = new EnumToNumberConverter<DepartmentType, short>();
            builder.Property(n => n.ReceiverDepartments).HasConversion(enumConverter);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text.Json;
using WebApi.Domain.Users;

namespace WebApi.Infrastructure.Data.EntityConfigurations
{
    internal class UserNotificationSettingConfiguration : IEntityTypeConfiguration<UserNotificationSetting>
    {
        private readonly JsonSerializerOptions _jsonSettings = new();

        public void Configure(EntityTypeBuilder<UserNotificationSetting> builder)
        {
            builder.HasKey(uns => uns.UserId);

            //builder.Property(uns => uns.MutedDepartmentsNotificationIds).HasConversion(
            //    v => JsonConvert.SerializeObject(v, _jsonSettings),
            //    v => JsonConvert.DeserializeObject<IEnumerable<Guid>>(v, _jsonSettings));
            builder.Property(uns => uns.MutedDepartmentsNotificationIds).HasConversion(
                v => JsonSerializer.Serialize(v, _jsonSettings),
                v => JsonSerializer.Deserialize<IEnumerable<Guid>>(v, _jsonSettings));

            builder.HasOne<User>().WithOne().HasForeignKey<UserNotificationSetting>(uns => uns.UserId);
        }
    }
}
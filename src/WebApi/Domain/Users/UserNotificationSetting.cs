using System;
using System.Collections.Generic;

namespace WebApi.Domain.Users
{
    public class UserNotificationSetting
    {
        public string UserId { get; set; }

        public IEnumerable<Guid> MutedDepartmentsNotificationIds { get; set; }
    }
}
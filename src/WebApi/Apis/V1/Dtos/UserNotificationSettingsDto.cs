using System;
using System.Collections.Generic;

namespace WebApi.Apis.V1
{
    public class UserNotificationSettingsDto
    {
        public IEnumerable<Guid> MutedDepartmentIds { get; set; }
    }
}
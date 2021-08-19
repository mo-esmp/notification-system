using System;
using System.Collections.Generic;

namespace WebClient.Models
{
    public class NotificationSettingsResponse : ResponseBase
    {
        public IEnumerable<Guid> MutedDepartmentIds { get; set; }
    }
}
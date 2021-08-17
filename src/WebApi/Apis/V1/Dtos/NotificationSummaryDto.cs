using System;

namespace WebApi.Apis.V1
{
    public class NotificationSummaryDto
    {
        public Guid Id { get; set; }

        public bool IseSeen { get; set; }

        public DateTime CreateDate { get; set; }

        public string Title { get; set; }
    }
}
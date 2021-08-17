using System;

namespace WebApi.Domain.Users
{
    public class UserNotification
    {
        public Guid NotificationId { get; set; }

        public string UserId { get; set; }

        public bool IsSeen { get; set; }

        public bool IsMuted { get; set; }
    }
}
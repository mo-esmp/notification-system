using System;
using System.Text.Json.Serialization;

namespace WebApi.Apis.V1
{
    public class NotificationDto
    {
        public Guid Id { get; set; }

        public DateTime CreateDate { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        [JsonIgnore]
        public bool IsSeen { get; set; }
    }
}
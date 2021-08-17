using System;

namespace WebApi.Domain.Notifications
{
    public record NotificationCreatedEvent(Guid NotificationId) : DomainEvent;
}
using MediatR;
using System;

namespace WebApi.Domain
{
    public abstract record DomainEvent : INotification
    {
        protected DomainEvent()
        {
            DateOccurred = DateTime.Now;
        }

        public DateTime DateOccurred { get; protected set; }
    }
}
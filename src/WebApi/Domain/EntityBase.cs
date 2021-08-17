using System.Collections.Generic;

namespace WebApi.Domain
{
    public abstract class EntityBase
    {
        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
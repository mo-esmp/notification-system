using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Domain;
using WebApi.Domain.Departments;
using WebApi.Domain.Notifications;
using WebApi.Domain.Users;

namespace WebApi.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        public ApplicationDbContext(IConfiguration configuration, IMediator mediator)
        {
            _configuration = configuration;
            _mediator = mediator;
        }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<UserNotification> UserNotifications { get; set; }

        public DbSet<UserNotificationSetting> UserNotificationSettings { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            await DispatchEvents();

            return result;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            foreach (var entity in builder.Model.GetEntityTypes().Where(e => e.ClrType.BaseType == typeof(EntityBase)))
                builder.Entity(entity.Name).Ignore(nameof(EntityBase.DomainEvents));
        }

        private async Task DispatchEvents()
        {
            var domainEventEntity = ChangeTracker
                .Entries<EntityBase>()
                .Select(x => x.Entity.DomainEvents)
                .SelectMany(x => x)
                .FirstOrDefault();

            if (domainEventEntity == null)
                return;

            await _mediator.Publish(domainEventEntity);
        }
    }
}
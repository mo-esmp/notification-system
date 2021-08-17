using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WebApi.Infrastructure.Data
{
    public class PostgresApplicationDbContext : ApplicationDbContext
    {
        private readonly IConfiguration _configuration;

        public PostgresApplicationDbContext(IConfiguration configuration, IMediator mediator)
            : base(configuration, mediator)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Domain.Departments;
using WebApi.Domain.Users;

namespace WebApi.Infrastructure.Data.EntityConfigurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.FirstName).IsUnicode().HasMaxLength(64).IsRequired();
            builder.Property(u => u.LastName).IsUnicode().HasMaxLength(64).IsRequired();

            builder.HasOne<Department>().WithMany().HasForeignKey(d => d.DepartmentId);
        }
    }
}
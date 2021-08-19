using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Apis.V1;
using WebApi.Domain.Departments;
using WebApi.Infrastructure.Data;

namespace WebApi.Infrastructure.QueryHandlers.V1
{
    public class DepartmentQueryHandler : IRequestHandler<DepartmentsListQuery, IEnumerable<Department>>
    {
        private readonly ApplicationDbContext _context;

        public DepartmentQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> Handle(DepartmentsListQuery request, CancellationToken cancellationToken)
        {
            var departmentType = await (from user in _context.Users
                                        join department in _context.Departments on user.DepartmentId equals department.Id
                                        where user.Id == request.UserId
                                        select department.Type).FirstOrDefaultAsync(cancellationToken);

            var availableTypes = GetDepartmentTypes(departmentType);
            var departments = await _context.Departments
                .AsNoTracking()
                .Where(d => availableTypes.Contains(d.Type))
                .ToListAsync(cancellationToken);

            return departments;
        }

        private DepartmentType[] GetDepartmentTypes(DepartmentType type)
        {
            return type switch
            {
                DepartmentType.HumanResources => new[] { DepartmentType.HumanResources },
                DepartmentType.DevOps => new[] { DepartmentType.HumanResources },
                DepartmentType.Development => new[] { DepartmentType.HumanResources, DepartmentType.DevOps },
                DepartmentType.Management => new[] { DepartmentType.HumanResources, DepartmentType.DevOps, DepartmentType.Development, DepartmentType.Management },
                _ => throw new InvalidEnumArgumentException($"Department type {type} is not valid")
            };
        }
    }
}
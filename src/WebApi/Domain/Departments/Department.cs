using System;

namespace WebApi.Domain.Departments
{
    public class Department
    {
        public Guid Id { get; set; }

        public DepartmentType Type { get; set; }

        public string Name { get; set; }
    }
}
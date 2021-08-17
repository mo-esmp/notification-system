using System;

namespace WebApi.Domain.Departments
{
    public class Department
    {
        //public Department(Guid id, string name)
        //{
        //    Id = id != default ? id : throw new NullArgumentDomainException("Department identifier could not be null"); ;
        //    Name = name ?? throw new NullArgumentDomainException("Department name could not be null");
        //}

        public Guid Id { get; set; }

        public DepartmentType Type { get; set; }

        public string Name { get; set; }
    }
}
using System;

namespace WebApi.Domain.Departments
{
    [Flags]
    public enum DepartmentType
    {
        HumanResources = 1,
        Development = 2,
        DevOps = 4,
        Management = 8
    }
}
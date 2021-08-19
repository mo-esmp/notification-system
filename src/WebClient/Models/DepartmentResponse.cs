using System;

namespace WebClient.Models
{
    public class DepartmentResponse : ResponseBase
    {
        public Guid Id { get; set; }

        public DepartmentType Type { get; set; }

        public string Name { get; set; }
    }

    public enum DepartmentType
    {
        HumanResources = 1,
        Development = 2,
        DevOps = 4,
        Management = 8
    }
}
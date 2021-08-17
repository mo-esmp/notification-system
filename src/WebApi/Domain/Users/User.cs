using Microsoft.AspNetCore.Identity;
using System;

namespace WebApi.Domain.Users
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Guid DepartmentId { get; set; }

        public DateTime BirthDate { get; set; }
    }
}
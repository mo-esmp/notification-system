using System;
using WebApi.Domain.Departments;

namespace WebApi.Domain.Notifications
{
    public class Notification : EntityBase
    {
        public Guid Id { get; set; }

        public DateTime CreateDate { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public DepartmentType SenderDepartment { get; set; }

        public DepartmentType? ReceiverDepartments { get; set; }

        public string CreatorUserId { get; set; }
    }
}
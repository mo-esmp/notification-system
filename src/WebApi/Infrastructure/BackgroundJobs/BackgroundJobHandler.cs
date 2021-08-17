using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Domain.Departments;
using WebApi.Domain.Notifications;
using WebApi.Domain.Users;
using WebApi.Infrastructure.Data;

namespace WebApi.Infrastructure.BackgroundJobs
{
    public class BackgroundJobHandler : IBackgroundJobHandler
    {
        private readonly ApplicationDbContext _context;

        public BackgroundJobHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task HandleNotificationCreatedJobAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);
            if (notification.ReceiverDepartments == null)
                return;

            if (notification.ReceiverDepartments.Value.HasFlag(DepartmentType.HumanResources))
                await CreateUserNotifications(notificationId, DepartmentType.HumanResources);

            if (notification.ReceiverDepartments.Value.HasFlag(DepartmentType.DevOps))
                await CreateUserNotifications(notificationId, DepartmentType.DevOps);

            if (notification.ReceiverDepartments.Value.HasFlag(DepartmentType.Development))
                await CreateUserNotifications(notificationId, DepartmentType.Development);

            if (notification.ReceiverDepartments.Value.HasFlag(DepartmentType.Management))
                await CreateUserNotifications(notificationId, DepartmentType.Management);

            await _context.SaveChangesAsync();
        }

        public async Task HandleBirthDayNotificationJobAsync()
        {
            var userIds = await _context.Users
                 .Where(u => u.BirthDate.Month == DateTime.Today.Month && u.BirthDate.Day == DateTime.Today.Day)
                 .Select(u => u.Id)
                 .ToListAsync();

            if (!userIds.Any())
                return;

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Title = "Birthday message",
                CreateDate = DateTime.Now,
                Message = "Happy birthday to you",
                SenderDepartment = DepartmentType.HumanResources
            };

            var userNotifs = userIds.Select(id => new UserNotification
            {
                UserId = id,
                NotificationId = notification.Id
            }).ToList();

            _context.Notifications.Add(notification);
            _context.UserNotifications.AddRange(userNotifs);
            await _context.SaveChangesAsync();
        }

        private Task<Department> GetDepartmentAsync(DepartmentType type)
        {
            return _context.Departments.AsNoTracking().FirstOrDefaultAsync(d => d.Type == type);
        }

        private async Task<List<UserWithNotifSettings>> GetUsersByDepartmentAsync(Guid departmentId)
        {
            var users = await (from user in _context.Users
                               join setting in _context.UserNotificationSettings on user.Id equals setting.UserId
                               where user.DepartmentId == departmentId
                               select new UserWithNotifSettings
                               {
                                   UserId = user.Id,
                                   MutedDepartmentsNotificationIds = setting.MutedDepartmentsNotificationIds
                               }).ToListAsync();

            return users;
        }

        private async Task CreateUserNotifications(Guid notificationId, DepartmentType departmentType)
        {
            var department = await GetDepartmentAsync(departmentType);
            var users = await GetUsersByDepartmentAsync(department.Id);

            if (!users.Any())
                return;

            var userNotifications = users.Select(user => new UserNotification
            {
                IsMuted = user.MutedDepartmentsNotificationIds.Any(n => n == department.Id),
                NotificationId = notificationId,
                UserId = user.UserId,
            }).ToList();

            _context.UserNotifications.AddRange(userNotifications);
        }

        private class UserWithNotifSettings
        {
            public string UserId { get; set; }

            public IEnumerable<Guid> MutedDepartmentsNotificationIds { get; set; }
        }
    }
}
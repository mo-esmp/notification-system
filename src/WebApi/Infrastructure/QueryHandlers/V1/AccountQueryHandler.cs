using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Apis.V1;
using WebApi.Domain.Departments;
using WebApi.Infrastructure.Data;

namespace WebApi.Infrastructure.QueryHandlers
{
    public class AccountQueryHandler :
        IRequestHandler<UserNotificationQuery, NotificationDto>,
        IRequestHandler<UserNotificationSettingsQuery, UserNotificationSettingsDto>,
        IRequestHandler<UserUnMutedNotificationSummariesQuery, IEnumerable<NotificationSummaryDto>>,
        IRequestHandler<UserMutedNotificationSummariesQuery, IEnumerable<NotificationSummaryDto>>
    {
        private readonly ApplicationDbContext _context;

        public AccountQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationDto> Handle(UserNotificationQuery request, CancellationToken cancellationToken)
        {
            var notification = await (from userNotif in _context.UserNotifications
                                      join notif in _context.Notifications on userNotif.NotificationId equals notif.Id
                                      where userNotif.UserId == request.UserId && notif.Id == request.NotificationId
                                      select new NotificationDto
                                      {
                                          CreateDate = notif.CreateDate,
                                          Id = notif.Id,
                                          Message = notif.Message,
                                          Title = notif.Title,
                                          IsSeen = userNotif.IsSeen
                                      }).FirstOrDefaultAsync(cancellationToken);

            return notification;
        }

        public async Task<UserNotificationSettingsDto> Handle(UserNotificationSettingsQuery request, CancellationToken cancellationToken)
        {
            var ids = await _context.UserNotificationSettings
                .Where(u => u.UserId == request.UserId)
                .Select(u => u.MutedDepartmentsNotificationIds)
                .FirstOrDefaultAsync(cancellationToken);

            return new() { MutedDepartmentIds = ids };
        }

        public Task<IEnumerable<NotificationSummaryDto>> Handle(UserUnMutedNotificationSummariesQuery request, CancellationToken cancellationToken)
        {
            return GetNotificationsSummary(request.UserId, request.DepartmentType, false);
        }

        public Task<IEnumerable<NotificationSummaryDto>> Handle(UserMutedNotificationSummariesQuery request, CancellationToken cancellationToken)
        {
            return GetNotificationsSummary(request.UserId, request.DepartmentType, true);
        }

        private async Task<IEnumerable<NotificationSummaryDto>> GetNotificationsSummary(string userId, DepartmentType type, bool isMuted)
        {
            var notifications = await (from userNotif in _context.UserNotifications
                                       join notif in _context.Notifications on userNotif.NotificationId equals notif.Id
                                       where userNotif.UserId == userId &&
                                             notif.SenderDepartment == type &&
                                             userNotif.IsMuted == isMuted
                                       orderby notif.CreateDate descending
                                       select new NotificationSummaryDto
                                       {
                                           CreateDate = notif.CreateDate,
                                           Id = notif.Id,
                                           Title = notif.Title,
                                           IseSeen = userNotif.IsSeen
                                       }).ToListAsync();

            return notifications;
        }
    }
}
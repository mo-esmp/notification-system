using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Apis.V1;
using WebApi.Domain.Departments;
using WebApi.Domain.Notifications;
using WebApi.Infrastructure.Data;

namespace WebApi.Infrastructure.CommandHandlers.V1
{
    public class NotificationCreateCommandHandler : IRequestHandler<NotificationCreateCommand>
    {
        private readonly ApplicationDbContext _context;

        public NotificationCreateCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(NotificationCreateCommand request, CancellationToken cancellationToken)
        {
            var departmentType = await (from usr in _context.Users
                                        join depart in _context.Departments on usr.DepartmentId equals depart.Id
                                        where usr.Id == request.UserId
                                        select depart.Type)
                                 .FirstOrDefaultAsync(cancellationToken);

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                CreateDate = DateTime.Now,
                Title = request.Title,
                Message = request.Message,
                ReceiverDepartments = GetDepartmentTypes(departmentType),
                CreatorUserId = request.UserId
            };

            notification.DomainEvents.Add(new NotificationCreatedEvent(notification.Id));
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        private DepartmentType GetDepartmentTypes(DepartmentType type)
        {
            return type switch
            {
                DepartmentType.HumanResources => DepartmentType.HumanResources | DepartmentType.DevOps | DepartmentType.Development | DepartmentType.Management,
                DepartmentType.DevOps => DepartmentType.Development | DepartmentType.Management,
                DepartmentType.Development => DepartmentType.Management,
                DepartmentType.Management => DepartmentType.Management,
                _ => throw new InvalidEnumArgumentException($"Department type {type} is not valid")
            };
        }
    }
}
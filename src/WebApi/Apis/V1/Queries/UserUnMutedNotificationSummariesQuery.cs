using MediatR;
using System.Collections.Generic;
using WebApi.Domain.Departments;

namespace WebApi.Apis.V1
{
    public record UserUnMutedNotificationSummariesQuery(
            string UserId,
            DepartmentType DepartmentType

    ) : IRequest<IEnumerable<NotificationSummaryDto>>;
}
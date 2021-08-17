using MediatR;
using System;
using System.Collections.Generic;

namespace WebApi.Apis.V1
{
    public record UserNotificationSettingsUpdateCommand(
        IEnumerable<Guid> MutedDepartments

    ) : UserCommandBase, IRequest;
}
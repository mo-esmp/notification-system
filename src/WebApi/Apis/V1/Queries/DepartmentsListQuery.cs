using MediatR;
using System.Collections.Generic;
using WebApi.Domain.Departments;

namespace WebApi.Apis.V1
{
    public record DepartmentsListQuery(
        string UserId

    ) : IRequest<IEnumerable<Department>>;
}
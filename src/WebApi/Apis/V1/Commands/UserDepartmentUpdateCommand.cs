using MediatR;
using WebApi.Domain.Departments;

namespace WebApi.Apis.V1
{
    public record UserDepartmentUpdateCommand(
        DepartmentType DepartmentType

    ) : UserCommandBase, IRequest<string>;
}
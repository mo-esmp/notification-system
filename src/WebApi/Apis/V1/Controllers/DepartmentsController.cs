using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Domain.Departments;
using WebApi.Extensions;

namespace WebApi.Apis.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DepartmentsController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public DepartmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///   Gets the list available departments for the user
        /// </summary>
        /// <response code="200">Department list</response>
        [HttpGet]
        public async Task<IEnumerable<Department>> Get()
        {
            var userId = User.GetId();
            var departments = await _mediator.Send(new DepartmentsListQuery(userId));

            return departments;
        }
    }
}
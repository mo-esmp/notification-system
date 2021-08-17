using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApi.Extensions;

namespace WebApi.Apis.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class NotificationsController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///   Create and publish a new notification
        /// </summary>
        /// <response code="200">200 status code</response>
        ///<response code="400">If the posted body is not valid or server side validation failed</response>
        [HttpPost]
        public async Task<IActionResult> Post(NotificationCreateCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            command.UserId = User.GetId();
            await _mediator.Send(command);

            return Ok();
        }
    }
}
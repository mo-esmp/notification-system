using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Domain.Departments;
using WebApi.Extensions;
using WebApi.Infrastructure.Swagger;

namespace WebApi.Apis.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountsController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///   Gets the list of user unmuted notifications
        /// </summary>
        /// <remarks>
        /// Department types
        ///
        ///     enum departmentTypes
        ///     {
        ///         humanResources = 1,
        ///         development = 2,
        ///         devOps = 4,
        ///         management = 8
        ///     }
        /// </remarks>
        /// <response code="200">List of unmuted notifications</response>
        [HttpGet("self/notifications/unmuted")]
        public async Task<IEnumerable<NotificationSummaryDto>> GetUnMutedNotifs(DepartmentType department)
        {
            UserUnMutedNotificationSummariesQuery query = new(User.GetId(), department);
            var notifications = await _mediator.Send(query);

            return notifications;
        }

        /// <summary>
        ///   Gets the list of user muted notifications
        /// </summary>
        /// <response code="200">List of muted notifications</response>
        [HttpGet("self/notifications/muted")]
        public async Task<IEnumerable<NotificationSummaryDto>> GetMutedNotifs(DepartmentType department)
        {
            UserUnMutedNotificationSummariesQuery query = new(User.GetId(), department);
            var notifications = await _mediator.Send(query);

            return notifications;
        }

        /// <summary>
        ///   Gets the user notification by notification identifier
        /// </summary>
        /// <response code="200">List of muted notifications</response>
        [HttpGet("self/notifications/{notifId:guid}")]
        public async Task<NotificationDto> GetNotif(Guid notifId)
        {
            var userId = User.GetId();
            var notification = await _mediator.Send(new UserNotificationQuery(userId, notifId));

            if (!notification.IsSeen)
                await _mediator.Send(new UserNotificationSeenUpdateCommand(userId, notifId));

            return notification;
        }

        /// <summary>
        ///   Login user via email and password
        /// </summary>
        /// <remarks>
        /// Replace "i" with number 1 to 40. First 5 accounts (1-5) are related to HR,
        /// next 25 accounts (6-30) are related to Development, next 5 accounts (31-35) are related to DevOps and
        /// last 5 accounts (36-40) are related to Management department.
        ///
        ///     email: user{i}@sample.com
        ///     password: 123@qwe
        ///
        /// </remarks>
        /// <response code="200">JWT token</response>
        ///<response code="400">If the posted body is not valid or server side validation failed</response>
        [HttpPost("login"), AllowAnonymous]
        [SwaggerRequestExample(typeof(LoginCommand), typeof(AccountLoginExampleValue))]
        public async Task<ActionResult<LoginDto>> Post(LoginCommand command)
        {
            if (!ModelState.IsValid)
                return ValidationProblem();

            var result = await _mediator.Send(command);

            return result;
        }

        /// <summary>
        ///   Update user current password to new password
        /// </summary>
        /// <response code="200">200 status code</response>
        ///<response code="400">If the posted body is not valid or server side validation failed</response>
        [HttpPost("self/change-password")]
        public async Task<IActionResult> Post(PasswordChangeCommand command)
        {
            if (!ModelState.IsValid)
                return ValidationProblem();

            command.UserId = User.GetId();
            await _mediator.Send(command);

            return Ok();
        }

        /// <summary>
        ///   Update user notification settings
        /// </summary>
        /// <response code="200">200 status code</response>
        ///<response code="400">If the posted body is not valid or server side validation failed</response>
        [HttpPut("self/notifications")]
        public async Task<IActionResult> Put(UserNotificationSettingsUpdateCommand command)
        {
            if (!ModelState.IsValid)
                return ValidationProblem();

            command.UserId = User.GetId();
            await _mediator.Send(command);

            return Ok();
        }

        /// <summary>
        ///   Change user department
        /// </summary>
        /// <response code="200">200 status code</response>
        ///<response code="400">If the posted body is not valid or server side validation failed</response>
        [HttpPut("{id}/departments")]
        [Authorize(Policy = "ManagementOnly")]
        public async Task<IActionResult> Put(string id, UserDepartmentUpdateCommand command)
        {
            if (!ModelState.IsValid)
                return ValidationProblem();

            command.UserId = id;
            var message = await _mediator.Send(command);

            NotificationCreateCommand createNotfiCommand = new("Department Change", message);
            createNotfiCommand.UserId = User.GetId();
            await _mediator.Send(createNotfiCommand);

            return Ok();
        }
    }
}
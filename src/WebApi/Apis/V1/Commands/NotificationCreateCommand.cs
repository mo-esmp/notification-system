using MediatR;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Apis.V1
{
    public record NotificationCreateCommand(
            [Required, MaxLength(64)] string Title,

            [Required, MaxLength(512)] string Message

    ) : UserCommandBase, IRequest;
}
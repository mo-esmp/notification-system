using MediatR;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Apis.V1
{
    public record LoginCommand(
        [Required, MaxLength(128)] string Email,

        [Required, MaxLength(64)] string Password

    ) : IRequest<LoginDto>;
}
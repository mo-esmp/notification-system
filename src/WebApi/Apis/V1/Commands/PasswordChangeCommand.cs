using MediatR;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Apis.V1
{
    public record PasswordChangeCommand(
        [Required, MaxLength(64)] string CurrentPassword,

        [Required, MaxLength(64)] string NewPassword,

        [property: Compare("NewPassword"), MaxLength(64)]
        string ConfirmNewPassword

     ) : UserCommandBase, IRequest;
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebClient.Models;

namespace WebClient.Services.Contracts
{
    public interface IAccountService
    {
        Task<LoginResponse> LoginAsync(LoginModel login);

        Task<ResponseBase> ChangePasswordAsync(ChangePasswordModel changePassword);

        Task<NotificationSettingsResponse> GetNotificationSettingsAsync();

        Task<ResponseBase> SaveNotificationSettingsAsync(IEnumerable<Guid> departmentIds);

        Task LogoutAsync();
    }
}
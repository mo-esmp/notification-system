using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebClient.Models;
using WebClient.Services.Contracts;
using WebClient.Services.HttpClients;

namespace WebClient.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly AccountHttpClient _client;
        private readonly ILocalStorageService _localStorageService;
        private readonly AppAuthenticationStateProvider _authStateProvider;

        public AccountService(
            AccountHttpClient client,
            ILocalStorageService localStorageService,
            AppAuthenticationStateProvider authStateProvider)
        {
            _client = client;
            _localStorageService = localStorageService;
            _authStateProvider = authStateProvider;
        }

        public async Task<LoginResponse> LoginAsync(LoginModel login)
        {
            try
            {
                var response = await _client.LoginAsync(login);
                if (!response.Succeeded)
                    return response;

                await _localStorageService.SetItemAsync("authToken", response.JwtToken);
                _authStateProvider.NotifyUserAuthentication(response.JwtToken);
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<ResponseBase> ChangePasswordAsync(ChangePasswordModel changePassword)
        {
            var response = await _client.ChangePasswordAsync(changePassword);

            return response;
        }

        public Task<NotificationSettingsResponse> GetNotificationSettingsAsync()
        {
            return _client.GetNotificationSettingsAsync();
        }

        public async Task<ResponseBase> SaveNotificationSettingsAsync(IEnumerable<Guid> departmentIds)
        {
            var model = new { MutedDepartmentIds = departmentIds };
            var response = await _client.SaveNotificationSettingsAsync(model);

            return response;
        }

        public async Task LogoutAsync()
        {
            await _localStorageService.RemoveItemAsync("authToken");
            _authStateProvider.NotifyUserLogout();
            _client.RemoveAuthHeader();
        }
    }
}
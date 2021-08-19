using Blazored.LocalStorage;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebClient.Models;

namespace WebClient.Services.HttpClients
{
    public class AccountHttpClient : HttpClientBase
    {
        public AccountHttpClient(HttpClient client, ILocalStorageService localStorageService)
            : base(client, localStorageService)
        {
        }

        public async Task<LoginResponse> LoginAsync(LoginModel login)
        {
            var response = await Client.PostAsJsonAsync("api/v1/accounts/login", login);
            if (!response.IsSuccessStatusCode)
                return await SetResponseErrorAsync<LoginResponse>(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LoginResponse>(content);
        }

        public async Task<ResponseBase> ChangePasswordAsync(ChangePasswordModel changePassword)
        {
            await SetAuthHeaderAsync();
            var response = await Client.PostAsJsonAsync("api/v1/accounts/self/change-password", changePassword);
            if (response.IsSuccessStatusCode)
                return new();

            return await SetResponseErrorAsync<ResponseBase>(response);
        }

        public async Task<NotificationSettingsResponse> GetNotificationSettingsAsync()
        {
            await SetAuthHeaderAsync();
            var response = await Client.GetFromJsonAsync<NotificationSettingsResponse>("api/v1/accounts/self/notification-settings");

            return response;
        }

        public async Task<ResponseBase> SaveNotificationSettingsAsync(object settings)
        {
            await SetAuthHeaderAsync();
            var response = await Client.PutAsJsonAsync("api/v1/accounts/self/notification-settings", settings);
            if (response.IsSuccessStatusCode)
                return new();

            return await SetResponseErrorAsync<ResponseBase>(response);
        }

        public void RemoveAuthHeader()
        {
            Client.DefaultRequestHeaders.Authorization = null;
        }
    }
}
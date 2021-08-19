using Blazored.LocalStorage;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebClient.Models;

namespace WebClient.Services.HttpClients
{
    public class HttpClientBase
    {
        private readonly ILocalStorageService _localStorageService;
        protected readonly HttpClient Client;

        public HttpClientBase(HttpClient client, ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
            Client = client;
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected async Task SetAuthHeaderAsync()
        {
            if (Client.DefaultRequestHeaders.Authorization != null)
                return;

            var token = await _localStorageService.GetItemAsync<string>("authToken");
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        }

        protected async Task<T> SetResponseErrorAsync<T>(HttpResponseMessage response) where T : ResponseBase, new()
        {
            if (response.StatusCode == HttpStatusCode.InternalServerError)
                return ResponseBase.CreateInternalServerError<T>();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return ResponseBase.CreateUnAuthorizedError<T>();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(content);
            result ??= ResponseBase.CreateBadRequestError<T>();

            return result;
        }
    }
}
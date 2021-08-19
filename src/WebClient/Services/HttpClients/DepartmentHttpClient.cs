using Blazored.LocalStorage;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebClient.Models;

namespace WebClient.Services.HttpClients
{
    public class DepartmentHttpClient : HttpClientBase
    {
        public DepartmentHttpClient(HttpClient client, ILocalStorageService localStorageService)
            : base(client, localStorageService)
        {
        }

        public async Task<IEnumerable<DepartmentResponse>> GetDepartmentsAsync()
        {
            await SetAuthHeaderAsync();
            var departments = await Client.GetFromJsonAsync<IEnumerable<DepartmentResponse>>("api/v1/departments");

            return departments;
        }
    }
}
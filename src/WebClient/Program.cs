using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebClient.Services.Contracts;
using WebClient.Services.HttpClients;
using WebClient.Services.Implementations;

namespace WebClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            Console.WriteLine($"Host base url {builder.HostEnvironment.BaseAddress}");

            //Uri baseAddress = new Uri(builder.HostEnvironment.BaseAddress);
            Uri baseAddress = new("http://localhost:5000");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = baseAddress });
            builder.Services.AddHttpClient<AccountHttpClient>(client => client.BaseAddress = baseAddress);
            builder.Services.AddHttpClient<DepartmentHttpClient>(client => client.BaseAddress = baseAddress);
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AppAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider, AppAuthenticationStateProvider>();
            builder.Services.AddScoped<IAccountService, AccountService>();

            await builder.Build().RunAsync();
        }
    }
}
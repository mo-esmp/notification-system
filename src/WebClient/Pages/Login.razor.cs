using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using WebClient.Models;
using WebClient.Services.Contracts;

namespace WebClient.Pages
{
    public partial class Login
    {
        private LoginModel LoginModel { get; set; } = new();

        [Inject]
        public IAccountService AccountService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public string Error { get; set; }

        public async Task OnSubmit()
        {
            var result = await AccountService.LoginAsync(LoginModel);

            if (!result.Succeeded)
                Error = result.GetErrorMessages;
            else
                NavigationManager.NavigateTo("/", true);
        }
    }
}
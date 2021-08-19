using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using WebClient.Services.Contracts;

namespace WebClient.Pages
{
    public partial class Logout
    {
        [Inject]
        public IAccountService AccountService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await AccountService.LogoutAsync();
            NavigationManager.NavigateTo("/", true);
        }
    }
}
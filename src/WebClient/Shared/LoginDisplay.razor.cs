using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebClient.Services.Implementations;

namespace WebClient.Shared
{
    public partial class LoginDisplay
    {
        private string Name { get; set; }

        [Inject]
        private AppAuthenticationStateProvider AuthStateProvider { get; set; }

        [Inject]
        private NavigationManager Navigation { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity is { IsAuthenticated: true })
            {
                var firstName = authState.User.Claims.First(c => c.Type == ClaimTypes.GivenName).Value;
                var lastName = authState.User.Claims.First(c => c.Type == ClaimTypes.Surname).Value;
                Name = $"{firstName} {lastName}";
            }

            await base.OnInitializedAsync();
        }

        private void BeginLogOut(MouseEventArgs args)
        {
            Navigation.NavigateTo("/logout");
        }
    }
}
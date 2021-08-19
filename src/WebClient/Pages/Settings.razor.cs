using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebClient.Models;
using WebClient.Services.Contracts;
using WebClient.Services.HttpClients;
using WebClient.Services.Implementations;

namespace WebClient.Pages
{
    public partial class Settings
    {
        private bool IsTaskRunning { get; set; }

        private string Error { get; set; }

        private string Error2 { get; set; }

        private string Success { get; set; }

        private string Success2 { get; set; }

        private IEnumerable<DepartmentResponse> Departments = new List<DepartmentResponse>();

        private List<Guid> MutedDepartmentIds { get; set; }

        private ChangePasswordModel ChangePasswordModel { get; set; } = new();

        [Inject] private IAccountService AccountService { get; set; }

        [Inject] private DepartmentHttpClient DepartmentHttpClient { get; set; }

        [Inject] private AppAuthenticationStateProvider AuthStateProvider { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var departmentTask = DepartmentHttpClient.GetDepartmentsAsync();
            var mutedDepartmentsTask = AccountService.GetNotificationSettingsAsync();
            await Task.WhenAll(departmentTask, mutedDepartmentsTask);

            Departments = await departmentTask;
            MutedDepartmentIds = (await mutedDepartmentsTask).MutedDepartmentIds?.ToList() ?? new List<Guid>(0);
            Console.WriteLine($"Dep length: {Departments.Count()}");

            await base.OnInitializedAsync();
        }

        private async Task OnChangePasswordSubmit()
        {
            IsTaskRunning = true;
            var result = await AccountService.ChangePasswordAsync(ChangePasswordModel);

            if (!result.Succeeded)
            {
                Error = result.GetErrorMessages;
                Success = null;
                IsTaskRunning = false;
                return;
            }

            Success = "Password changed successfully.";
            Error = null;
            ChangePasswordModel = new();
            IsTaskRunning = false;
        }

        private async Task OnChangeMuteSettingsSubmit()
        {
            IsTaskRunning = true;
            var result = await AccountService.SaveNotificationSettingsAsync(MutedDepartmentIds);

            if (!result.Succeeded)
            {
                Error2 = result.GetErrorMessages;
                Success2 = null;
                IsTaskRunning = false;
                return;
            }

            Success2 = "Notification mute setting changed successfully.";
            Error2 = null;
            IsTaskRunning = false;
        }

        private void CheckboxChanged(ChangeEventArgs e, Guid id)
        {
            if (MutedDepartmentIds.Contains(id))
            {
                MutedDepartmentIds.Remove(id);
            }
            else
            {
                MutedDepartmentIds.Add(id);
            }
        }
    }
}
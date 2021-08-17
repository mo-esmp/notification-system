using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace WebApi.Apis.V1
{
    public abstract record UserCommandBase
    {
        [JsonIgnore, BindNever]
        public string UserId { get; set; }
    }
}